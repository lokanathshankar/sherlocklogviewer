import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { ColumnComponent, ColumnDefinition, Filter, FilterType } from 'tabulator-tables';
import { MessageHelper } from '../../services/messagehelper';
import { ThemeService } from '../../services/theme-service.ts';
import { UiSessionService } from '../../services/uisession.service';
import { ColumnEvents } from '../column-options/ColumnEvents';
import { MessagingConstants, SharedDataFactory } from '../FacsAndConsts';
import { TableEvents } from '../table-view/TableEvents';
import { FilterEvents, FilterWith, FilterWithWhat } from './FilterEvents';
import { v4 as uuidv4 } from 'uuid';

@Component({
  selector: 'app-filter-view',
  templateUrl: './filter-view.component.html',
  styleUrls: ['./filter-view.component.css'],
})
export class FilterViewComponent implements OnInit {
  private readonly myDataContextShared = SharedDataFactory.Instance.getSharedDataContextRows("FilterViewComponent");
  private myFilterMap: Map<string, string> = new Map([
    [FilterWith.Like, 'like'],
    [FilterWith.Equals, '='],
    [FilterWith.NotEquals, '!='],
    [FilterWith.Regex, 'regex'],
  ]);
  public colors: string[] = ['PowderBlue', 'LightGreen', 'Beige', 'LightCyan', 'LightCoral', 'LightSkyBlue', 'Linen', 'MistyRose', 'PaleGreen', 'PeachPuff', 'LightGreen', 'Pink'];
  public colorsDark: string[] = ['#1F1F1F', '#2C2C2C', '#444444', '#666666', '#888888', '#FF5733', '#FFC300', '#36A2EB', '#4BC0C0', '#FF6384', '#6A4F4D', '#FFCC29',];

  public filterText: string | null = null;
  public filterWith: FilterWith = FilterWith.Like;
  public currentColumn: string | null = null;
  public readonly columnFilters: FilterWithWhat[] = this.myDataContextShared.rows;
  public avaliableColumns: string[] = []
  public avaliableFilters: string[] = Array.from(this.myFilterMap.keys());
  isDarkTheme = false;
  constructor(
    private messageService: MessageHelper,
    private myTableEvents: TableEvents,
    private myColumnEvents: ColumnEvents,
    private mySessionService: UiSessionService,
    private myFilterEvents: FilterEvents,
    private themeService: ThemeService,

    private cdRef: ChangeDetectorRef) {
    this.themeService.themeDark$.subscribe((isDark) => {
      this.isDarkTheme = isDark;
    });
    myTableEvents.columnChanges.subscribe((theX: ColumnComponent[]) => {
      this.avaliableColumns.length = 0;
      theX.forEach((theX) => {
        if (!theX.isVisible()) {
          return;
        }

        this.avaliableColumns.push(theX.getField());
      });
    });

    myColumnEvents.hideColumn.subscribe((theColumnName: string) => {
      const aToHide = this.avaliableColumns.indexOf(theColumnName);
      if (aToHide == -1) {
        return;
      }

      this.avaliableColumns.splice(aToHide, 1);
    });

    myColumnEvents.showColumn.subscribe((theColumnName: string) => {
      const aToHide = this.avaliableColumns.indexOf(theColumnName);
      if (aToHide != -1) {
        return;
      }

      this.avaliableColumns.push(theColumnName);
    });

    this.mySessionService.sessionReset.subscribe(() => {
      this.avaliableColumns = []
    });
  }

  onRemoveFilter(theFilterToRemove: FilterWithWhat): void {
    this.myDataContextShared.RemoveRow(theFilterToRemove);
    this.myFilterEvents.applyFilter.next(this.columnFilters);
  }

  onResetFilters(): void {
    this.myFilterEvents.clearAllFilters.next();
    this.myDataContextShared.ClearRows();
    this.myFilterEvents.applyFilter.next(this.columnFilters);
  }
  onGroupFilters(): void {
    const newGuid = uuidv4();
    console.log("GUID" + newGuid);
    //Removing alreadu used color from list for importing filter case
    this.columnFilters.forEach((theX: FilterWithWhat) => {
      if (this.isDarkTheme) {
        if (theX.Color == this.colorsDark[0]) {
          this.colorsDark.shift();
        }
      }
      else {
        if (theX.Color == this.colors[0]) {
          this.colors.shift();
        }
      }
    });

    this.columnFilters.forEach((theX: FilterWithWhat) => {

      if (theX.Or === true && theX.Grouped === false) {
        theX.Guid = newGuid;
        if (this.isDarkTheme) {
          theX.Color = this.colorsDark[0];
        }
        else {
          theX.Color = this.colors[0];
        }
        theX.Grouped = true;
      }
    });
    if (this.isDarkTheme) {
      this.colorsDark.shift();
    }
    else {
      this.colors.shift();
    }
    this.myFilterEvents.applyFilter.next(this.columnFilters);
  }

  onApplyFilters(): void {
    this.myFilterEvents.applyFilter.next(this.columnFilters);

  }
  onAddNewFilter(): void {
    if (this.currentColumn == null) {
      this.messageService.PostError('Column must be selected for filtering...');
      return;
    }

    if (this.filterText == null) {
      this.messageService.PostError('Filter text must be entered...');
      return;
    }

    const aFilter = new FilterWithWhat();
    aFilter.column = this.currentColumn;
    aFilter.what = this.filterText;
    aFilter.with = this.myFilterMap.get(this.filterWith);
    aFilter.Or = false;
    this.columnFilters.push(aFilter);
    this.myFilterEvents.applyFilter.next(this.columnFilters);
  }
  onDownloadFilters() {
    // Get the current applied filters
    const appliedFilters = this.columnFilters;

    // Convert the filter data to JSON format
    const filterData = JSON.stringify(appliedFilters, null, 2);

    // Create a Blob containing the JSON data
    const blob = new Blob([filterData], { type: 'application/json' });

    // Create a temporary anchor element for downloading
    const a = document.createElement('a');
    a.href = window.URL.createObjectURL(blob);

    // Set the filename for the downloaded file (you can change this)
    a.download = 'applied_filters.json';

    // Trigger a click event on the anchor element to download the file
    a.click();

    // Clean up by revoking the object URL
    window.URL.revokeObjectURL(a.href);
  }
  onImportFilters(event: Event): void {
    const inputElement = event.target as HTMLInputElement;
    const file = (inputElement.files as FileList)[0];

    if (file) {
      const reader = new FileReader();

      reader.onload = (e) => {
          const importedFilters: FilterWithWhat[] = JSON.parse(e.target?.result as string);
          
          this.columnFilters.push(...importedFilters);
          this.myFilterEvents.applyFilter.next(this.columnFilters);
      };
      this.cdRef.detectChanges();
      reader.readAsText(file);
    }
  }

  importFiltersClick(): void {
    const importFiltersInput = document.getElementById('importFiltersInput') as HTMLInputElement;
    importFiltersInput.click();
  }

  onChecked(columnFilter: any, event: any) {
    columnFilter.Or = event.target.checked;
  }
  getColor(aColoumObj: any) {
    return aColoumObj.Color;
  }
  ngOnInit(): void {
  }
}
