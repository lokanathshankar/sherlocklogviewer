import { Component, OnInit, Input } from '@angular/core';
import { ColumnComponent, ColumnDefinition, RowComponent, TabulatorFull } from 'tabulator-tables';
import { Engine } from '../../services/engine.service';
import { Logger } from '../../services/logger.service';
import { TableRenderer } from '../../services/render.service';
import { UiSessionService } from '../../services/uisession.service';
import { timer, Tracer } from '../../utils/tracer';
import { ColumnEvents } from '../column-options/ColumnEvents';
import { TableConstants, TableFactory } from '../FacsAndConsts';
import { FindEvents } from '../find-view/FindEvents';
import { TableShellEvents } from '../table-shell/TableShellEvents';
import { TableColumns, TableColumnUtil, TableDataProvider, TableEvents, TableUtils } from '../table-view/TableEvents';

@Component({
  selector: 'app-info-area',
  templateUrl: './info-area.component.html',
  styleUrls: ['./info-area.component.css']
})
export class InfoAreaComponent implements OnInit {
  private myDomain: string = "InfoAreaComponent";
  public SelectedLog: string = ""
  public activeIndex: number = 1;
  private myBookmarkTable?: TabulatorFull;
  private myFindResultsTable?: TabulatorFull;
  private myBookMarkColumnUtils?: TableColumnUtil;
  private myFindResultColumnUtils?: TableColumnUtil;
  public rawTextHeight: string = "200px";
  constructor(

    private myLogger: Logger,
    private myTableEvents: TableEvents,
    private myTableRenderer: TableRenderer,
    private myColumnEvents: ColumnEvents,
    private myFindEvents: FindEvents,
    private mySessionService: UiSessionService,
    private myTableShellEvents: TableShellEvents) {
    const aTrace = new Tracer(this.myLogger, this.myDomain, "ngAfterViewInit");
    myTableEvents.tableRowHovered.subscribe((theLogId) => {
      this.Display(theLogId, this.myTableRenderer.Engine);
    });

    myTableEvents.handleBookmark.subscribe((theRow: RowComponent) => {
      if (this.myBookmarkTable?.getRow(theRow.getIndex())) {
        this.myBookmarkTable?.deleteRow(theRow.getIndex());
      }
      else {
        this.myBookmarkTable?.addData(theRow.getData());
        this.myBookmarkTable?.redraw(true)
      }
    });

    myTableEvents.headerChange.subscribe((theColumnConfig) => {
      InfoAreaComponent.ResetTableHeader(this.myBookmarkTable, theColumnConfig);
      InfoAreaComponent.ResetTableHeader(this.myFindResultsTable, theColumnConfig);
    });

    myTableEvents.headerRearranged.subscribe((theColumns: ColumnDefinition[]) => {
      this.myBookmarkTable?.setColumns(theColumns);
      this.myFindResultsTable?.setColumns(theColumns);
      TableUtils.HideDataColumns(this.myBookmarkTable);
      TableUtils.HideDataColumns(this.myFindResultsTable);
    });

    myTableEvents.findResults.subscribe((theRowArray: RowComponent[]) => {
      if (this.myFindResultsTable == null) {
        return
      }

      const aTimer = timer();
      const aLocalData: any[] = []
      theRowArray.forEach((theRow: RowComponent) => {
        aLocalData.push(theRow.getData());
      });

      this.myFindResultsTable.setData(aLocalData);
      this.myFindResultsTable.redraw();
      aTrace.info(`Populating Data Took ${aTimer.ms}`);
    });

    this.myFindEvents.clearAll.subscribe(() => {
      this.myFindResultsTable?.clearData();
    });

    this.mySessionService.sessionReset.subscribe(() => {
      this.myFindResultsTable?.clearData();
      this.myBookmarkTable?.clearData();
    });

    this.myTableShellEvents.infoAreaHeightChanged.subscribe((theX: number) => {
      this.rawTextHeight = `${(theX - 50).toString()}px`;
      this.myBookmarkTable?.setHeight(theX - 50);
      this.myFindResultsTable?.setHeight(theX - 50);
    });
  }
  private static ResetTableHeader(theTable: TabulatorFull | undefined, theColumnConfig: any) {
    theTable?.clearData();
    theTable?.setColumns(theColumnConfig);
    theTable?.hideColumn(TableColumns.Header);
    theTable?.hideColumn(TableColumns.ID);
  }
  ngOnInit(): void {
  }

  ngAfterViewInit(): void {
    this.myBookmarkTable = new TabulatorFull("#myBookmarkTable", {
      layout: 'fitData',
      index: TableColumns.ID,
      height: "300px",
      movableColumns: true,
      nestedFieldSeparator : false,
      placeholder: 'No bookmarks.', //display message to user on empty table
    });

    console.log(this.myBookmarkTable)
    TableFactory.Instance.RegisterTable(TableConstants.BookmarkTable, new TableDataProvider(this.myBookmarkTable));
    this.myFindResultsTable = new TabulatorFull("#myFindResultsTable", {
      layout: 'fitData',
      index: TableColumns.ID,
      height: "300px",
      movableColumns: true,
      nestedFieldSeparator: false,
      placeholder: 'No results to present.', //display message to user on empty table,
    });

    TableFactory.Instance.RegisterTable(TableConstants.FindResultTable, new TableDataProvider(this.myFindResultsTable));
    this.myBookMarkColumnUtils = new TableColumnUtil(this.myBookmarkTable, this.myColumnEvents);
    this.myBookMarkColumnUtils.Hold();
    this.myFindResultColumnUtils = new TableColumnUtil(this.myFindResultsTable, this.myColumnEvents);
    this.myFindResultColumnUtils.Hold();

    this.myBookmarkTable.on("rowClick", (_: Event, row: RowComponent) => {
      this.myTableEvents.navigateToRow.next(row.getIndex());
    });

    this.myFindResultsTable.on("rowClick", (_: Event, row: RowComponent) => {
      this.myTableEvents.navigateToRow.next(row.getIndex());
    });
  }

  activeIndexChange(theValue: Event): void {
    if (this.activeIndex === 1) {
      setTimeout(() => {
        this.myBookmarkTable?.redraw(true);
      }, 100);
    }

    if (this.activeIndex === 2) {
      setTimeout(() => {
        this.myFindResultsTable?.redraw(true)
      }, 100);
    }
  }
  private Display(theLogId: number, theEnding?: Engine): void {
    theEnding?.getRawLog(theLogId).subscribe((theString: string) => {
      this.SelectedLog = theString;
    });
  }
}
