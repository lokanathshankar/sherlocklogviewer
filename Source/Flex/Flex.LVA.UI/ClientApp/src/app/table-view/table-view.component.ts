import { Component, OnInit } from '@angular/core';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ColumnComponent, ColumnDefinition, Filter, RowComponent, Tabulator, TabulatorFull } from 'tabulator-tables';
import { LogDefinition } from '../../common/logdefinition';
import { ILogHeader, ISymanticLogs } from '../../common/loggraph';
import { Logger } from '../../services/logger.service';
import { TableRenderer } from '../../services/render.service';
import { timer, Tracer } from '../../utils/tracer';
import { ColumnEvents } from '../column-options/ColumnEvents';
import { FilterEvents, FilterWith, FilterWithWhat } from '../filter-view/FilterEvents';
import { FindEvents, FindWith, FindWithWhat } from '../find-view/FindEvents';
import { GlobalProgressBarComponent } from '../global-progress-bar/global-progress-bar.component';
import { GlobalProgressBarEvents } from '../global-progress-bar/GloblaProgressBarEvents';
import { ParserEventsAndUpdates } from '../parser-setup-view/ParserEvents';
import { MenuEvents } from '../top-menu/TopMenuEvents';
import { TableColumns, TableColumnUtil, TableDataProvider, TableEvents, TableUtils } from './TableEvents';
import * as YAML from "yaml";
import { MessageHelper } from '../../services/messagehelper';
import { TableConstants, TableFactory } from '../FacsAndConsts';
import { ParserSetupViewComponent } from '../parser-setup-view/parser-setup-view.component';
import { DialogService } from 'primeng/dynamicdialog';
import { UserWorkboxService } from '../../services/userworkbox.service';
import { Subject } from 'rxjs';
import { ParserLoaderComponent } from '../parser-loader/parser-loader.component';
import { TestDataLoader } from '../../utils/testdataloader';
import { LogDataType } from '../../common/enums';
import { UiSessionService } from '../../services/uisession.service';
import { SkipHeaderComponent } from '../skip-header/skip-header.component';
import { cwd } from 'process';
@Component({
  selector: 'app-table-view',
  templateUrl: './table-view.component.html',
  styleUrls: ['./table-view.component.css']
})
export class TableViewComponent implements OnInit {
  private isDialogOpen: boolean = false; // Variable to track whether the dialog is open

  private myLogPrepareWaitSubject: Subject<void> = new Subject<void>();
  private myDomain: string = "TableViewComponent";
  private myCurrentHeader?: ILogHeader;
  private myTable?: TabulatorFull;
  public columnConfig: any[];
  public tableData: any[];
  private myCurrentFindIndex: number = -1;
  private myPreviousFind: FindWithWhat = new FindWithWhat();
  private myCurrentFindResults: Set<RowComponent> = new Set<RowComponent>();
  private myCurrentFindResultsOrdered: Array<RowComponent> = new Array<RowComponent>();
  private myFilterMap: Map<FindWith, string> = new Map([
    [FindWith.Like, 'like'],
    [FindWith.Equals, '='],
    [FindWith.Regex, 'regex'],
  ]);
  private myFile?: File;
  constructor(
    private myTestDataLoader: TestDataLoader,
    private myUserBoxService: UserWorkboxService,
    private myConfirmService: ConfirmationService,
    private myLogger: Logger,
    private myRenderer: TableRenderer,
    private myTableEvents: TableEvents,
    private myMenuEvents: MenuEvents,
    private myFindEvents: FindEvents,
    private myProgressGlobal: GlobalProgressBarEvents,
    private myFilterEvents: FilterEvents,
    private myColumnEvents: ColumnEvents,
    private myParserConfigEvents: ParserEventsAndUpdates,
    private confirmationService: ConfirmationService,
    private myDialogService: DialogService,
    private mySessionService: UiSessionService,
    private myMessageService: MessageHelper) {
    const aTrace = new Tracer(this.myLogger, this.myDomain, "constructor");
    this.columnConfig = [];
    this.tableData = [];
    aTrace.info("Init Done");
  }

  ngAfterViewInit(): void {
    const aTrace = new Tracer(this.myLogger, this.myDomain, "ngAfterViewInit");
    this.myLogPrepareWaitSubject.subscribe(() => { this.TriggerPreparingOfLog() });
    this.myTable = new TabulatorFull("#myTable", {
      layout: 'fitData',
      index: TableColumns.ID,
      importFormat: "array",
      selectable: true,
      selectableRangeMode: "click",
      movableColumns: true,
      nestedFieldSeparator: false,
      placeholder: 'No data to show, use top file menu to load one.', //display message to user on empty table
      rowContextMenu: this.GetRowMenu(), //add context menu to rows
    });

    TableFactory.Instance.RegisterTable(TableConstants.MainTable, new TableDataProvider(this.myTable));
    new TableColumnUtil(this.myTable, this.myColumnEvents, this.myTableEvents);
    this.myTable.on("columnMoved", (column, columns) => {
      if (this.myTable == null) {
        return
      }

      this.myTableEvents.headerRearranged.next(this.myTable.getColumnDefinitions());
    });

    this.myTable.on("rowMouseEnter", (e: Event, row: RowComponent) => {
      this.myTableEvents.tableRowHovered.next(Number(row.getCell(TableColumns.ID).getValue()))
    });

    this.myTable.on("rowDblClick", (e: Event, row: RowComponent) => {
      this.myTableEvents.handleBookmark.next(row);
    });

    this.myTableEvents.navigateToRow.subscribe((theId: number) => {
      this.myTable?.scrollToRow(this.myTable.getRow(theId))
    });

    this.myMenuEvents.logFileSelected.subscribe((theFile: File) => {
      this.StartPrepareLogProcess(theFile);
    });

    this.myFindEvents.findNext.subscribe((theFindWithWhat: FindWithWhat) => {
      if (this.myPreviousFind.Equals(theFindWithWhat) != true) {
        this.PrepareFindResults(theFindWithWhat)
      }

      this.myPreviousFind = theFindWithWhat;
      this.myCurrentFindIndex++;
      this.ScrollToIndex()
    });

    this.myFindEvents.findPrevious.subscribe((theFindWithWhat: FindWithWhat) => {
      if (this.myPreviousFind.Equals(theFindWithWhat) != true) {
        this.PrepareFindResults(theFindWithWhat)
      }

      this.myPreviousFind = theFindWithWhat;
      this.myCurrentFindIndex--;
      this.ScrollToIndex()
    });

    this.myFindEvents.findAll.subscribe((theFindWithWhat: FindWithWhat) => {
      if (this.myPreviousFind.Equals(theFindWithWhat) == true) {
        return;
      }

      this.myTable?.alert("Finding data, please wait");
      this.myPreviousFind = theFindWithWhat;
      this.PrepareFindResults(theFindWithWhat)
      this.myTableEvents.findResults.next(this.myCurrentFindResultsOrdered);
      this.myTable?.clearAlert();
    });

    this.myFindEvents.clearAll.subscribe(() => {
      this.myPreviousFind = new FindWithWhat();
    });

    this.myFilterEvents.clearAllFilters.subscribe(() => {
      if (this.myTable == null) {
        return
      }

      TableFactory.Instance.GetMainTable()?.ClearFilters();
      TableFactory.Instance.GetFindTable()?.ClearFilters();
    });

    this.myFilterEvents.applyFilter.subscribe((theX: FilterWithWhat[]) => {
      const aTrace = new Tracer(this.myLogger, this.myDomain, "applyFilter");

      TableFactory.Instance.GetMainTable()?.ClearFilters();
      TableFactory.Instance.GetFindTable()?.ClearFilters();
      const aFinalFilter: any = [];
      const aDef = new FilterWithWhat();
      theX.forEach((theFilter) => {
        theFilter.Visited = false;
      });

      for (var aIndex = 0; aIndex < theX.length; aIndex++) {
        const aElement = theX[aIndex];
        if (aElement.Guid == aDef.Guid) {
          aElement.Visited = true;
          aFinalFilter.push({ field: aElement.column, type: aElement.with, value: aElement.what });
        }
        else {
          if (aElement.Visited) {
            continue;
          }

          const aSubArr = []
          aSubArr.push({ field: aElement.column, type: aElement.with, value: aElement.what })
          for (var aI = aIndex + 1; aI < theX.length; aI++) {
            const aInnderEle = theX[aI];
            if (aInnderEle.Guid == aElement.Guid) {
              aSubArr.push({ field: aInnderEle.column, type: aInnderEle.with, value: aInnderEle.what })
              aInnderEle.Visited = true;
            }
          }

          aFinalFilter.push(aSubArr);
        }
      }

      aTrace.info(aFinalFilter);
      TableFactory.Instance.GetMainTable()?.ApplyFilter(aFinalFilter);
      TableFactory.Instance.GetFindTable()?.ApplyFilter(aFinalFilter);

    });

    this.myMenuEvents.parserConfigClicked.subscribe((theFile: File) => {
      this.SetupParser(theFile);
    });

    this.myParserConfigEvents.parserConfigSet.subscribe((theX: LogDefinition) => {
      this.HandleSetOrUpdateOfParser(theX);
    });

    this.myParserConfigEvents.parserConfigChanged.subscribe((theX: LogDefinition) => {
      this.HandleSetOrUpdateOfParser(theX);
    });
  }

  private HandleSetOrUpdateOfParser(theX: LogDefinition): void {
    this.myRenderer.Engine?.SetLogDefinition(theX);
    if (this.myFile == null) {
      return;
    }

    if (theX.AutoDetected === true) {
      return;
    }

    this.confirmationService.confirm({
      message: "Parser config updated, do you want to re-load the logs?",
      icon: 'pi pi-refresh',
      header: "Refresh Logs...",
      accept: () => {
        this.StartPrepareLogProcess(this.myFile);
      }
    });
  }

  private StartPrepareLogProcess(theFile: File | undefined) {
    this.myFile = theFile;
    this.CleanupTableStuff();
    if (this.myFile?.name.endsWith('.json')) {
      if (this.isDialogOpen) {
        return;
      }
      this.isDialogOpen = true;
      const dialogRef = this.myDialogService.open(SkipHeaderComponent, {
        header: 'Sherlock Log Viewer',
        closable: false,
        closeOnEscape: false,
      });

      dialogRef.onClose.subscribe(result => {
        this.isDialogOpen = false;
        if (result !== undefined) {
          const aDef = new LogDefinition();
          aDef.LogFileType = LogDataType.Json;
          aDef.HeaderLineCount = result;
          this.myRenderer.Engine?.SetLogDefinition(aDef);
          this.myLogPrepareWaitSubject.next();
        }
      });
      return;
    }

    if (this.myFile?.name.endsWith('.xml')) {

      if (this.isDialogOpen) {
        return;
      }
      this.isDialogOpen = true;
      const dialogRef = this.myDialogService.open(SkipHeaderComponent, {
        header: 'Sherlock Log Viewer',
        closable: false,
        closeOnEscape: false,
      });

      dialogRef.onClose.subscribe(result => {
        this.isDialogOpen = false;
        if (result !== undefined) {
          const aDef = new LogDefinition();
          aDef.LogFileType = LogDataType.Xml;
          aDef.HeaderLineCount = result;
          this.myRenderer.Engine?.SetLogDefinition(aDef);
          this.myLogPrepareWaitSubject.next();
        }
      });
      return;
    }

    if (this.myFile?.name.endsWith('.csv') || this.myFile?.name.endsWith('.tsv')) {
      if (this.isDialogOpen) {
        return;
      }
      this.isDialogOpen = true;
      const dialogRef = this.myDialogService.open(SkipHeaderComponent, {
        header: 'Sherlock Log Viewer',
        closable: false,
        closeOnEscape: false,
      });

      dialogRef.onClose.subscribe(result => {
        this.isDialogOpen = false; 
        if (result !== undefined) {
          const aDef = new LogDefinition();
          aDef.LogFileType = LogDataType.Delimited;
          aDef.HeaderLineCount = result;
          this.myRenderer.Engine?.SetLogDefinition(aDef);
          this.myLogPrepareWaitSubject.next();
        }
      });
      return;
    }


    if (this.myFile?.name.endsWith('.evtx')) {
      const aDef = new LogDefinition();
      aDef.LogFileType = LogDataType.Evtx;
      this.myRenderer.Engine?.SetLogDefinition(aDef);
      this.myLogPrepareWaitSubject.next();
      return;
    }

    const aCurrentParser = this.myParserConfigEvents.GetCurrentParser();
    if (aCurrentParser == null) {
      this.StartPrepareLogProcessAndSetItUp();
    }
    else {
      aCurrentParser.LogFileType = LogDataType.PlainText;
      this.myRenderer.Engine?.SetLogDefinition(aCurrentParser);
      this.myLogPrepareWaitSubject.next();
    }
  }

  private TriggerPreparingOfLog(): void {
    if (this.myFile == null) { return }
    this.myProgressGlobal?.showProgress.next("Loading selected file, please wait");
    this.myTable?.clearData();
    this.myRenderer.Engine?.PrepareLogs(this.myFile);
  }

  private StartPrepareLogProcessAndSetItUp() {
    this.confirmationService.confirm({
      message: 'Do you want the application to try and automatically detect the format?',
      header: 'Auto detect?',
      icon: 'pi pi-exclamation-info',
      accept: () => {
        const aLodDef = new LogDefinition();
        aLodDef.LogFileType = LogDataType.Auto;
        this.myRenderer.Engine?.SetLogDefinition(aLodDef);
        this.myLogPrepareWaitSubject.next()
      },
      reject: () => {
        this.myDialogService.open(ParserLoaderComponent, { header: 'Parser Selection', maximizable: true, contentStyle: { overflow: 'auto' } });
      }
    });
  }

  private GetRowMenu(): any {
    let aRow = [
      {
        label: "<i class='fas fa-user'></i> Pin/Un-Pin to top",
        action: (e: any, row: RowComponent) => {
          if (row.isFrozen() == true) {
            row.unfreeze();
          }
          else {
            row.freeze();
          }
        }
      },
      {
        label: "<i class='fas fa-user'></i> Copy Raw Log",
        action: (e: any, row: RowComponent) => {
          this.myRenderer.Engine?.copyRawToClipBoard(Number(row.getCell(TableColumns.ID).getValue()))
        }
      }
    ]

    return aRow
  }

  private ScrollToIndex(): void {
    if (this.myCurrentFindResultsOrdered.length == 0) {
      return;
    }

    if (this.myCurrentFindIndex >= this.myCurrentFindResultsOrdered.length) {
      this.myCurrentFindIndex = 0;
    }

    if (this.myCurrentFindIndex < 0) {
      this.myCurrentFindIndex = this.myCurrentFindResultsOrdered.length - 1;
    }

    this.myCurrentFindResultsOrdered[this.myCurrentFindIndex].scrollTo();
  }

  private PrepareFindResults(theFindWithWhat: FindWithWhat): void {
    const aTrace = new Tracer(this.myLogger, this.myDomain, "PrepareFindResults");
    const aTimer = timer();
    if (this.myTable == null) {
      return;
    }

    this.myCurrentFindResultsOrdered = []
    this.myCurrentFindResults.clear();
    for (var aColumn of this.columnConfig) {
      if (aColumn.field === TableColumns.ID) {
        continue;
      }

      if (aColumn.field === TableColumns.Header) {
        continue;
      }

      const aFilterType = this.myFilterMap.get(theFindWithWhat.With);
      if (aFilterType == null) {
        return;
      }

      let aRows: RowComponent[] = [];
      switch (theFindWithWhat.With) {
        case FindWith.Equals:
          aRows = this.myTable.searchRows(aColumn.field, "=", theFindWithWhat.What);
          break;
        case FindWith.Like:
          aRows = this.myTable.searchRows(aColumn.field, "like", theFindWithWhat.What);
          break;
        case FindWith.Regex:
          aRows = this.myTable.searchRows(aColumn.field, "regex", theFindWithWhat.What);
          break;
      }

      aRows.forEach((theX, _) => {
        if (!this.myCurrentFindResults.has(theX)) {
          this.myCurrentFindResultsOrdered.push(theX);
        }
      });
    }

    aTrace.info(`Filtering Done At ${aTimer.ms}`);
  }

  ngOnInit(): void {
    const aTrace = new Tracer(this.myLogger, this.myDomain, "ngOnInit");
    aTrace.info(`Renderer Starting...`);
    this.myRenderer.Start();
    {
      this.myTestDataLoader.onTestHeaderReady.subscribe((theValue: ILogHeader) => {
        this.PrepareHeader(theValue);
      });

      this.myTestDataLoader.onTestDataReady.subscribe((theValue: ISymanticLogs) => {
        this.PrepareData(theValue);
        this.RenderTable();
      });
    }
    this.myRenderer.HeaderChanges().subscribe((theValue: ILogHeader) => {
      this.PrepareHeader(theValue);
    });

    this.myRenderer.DataChanges().subscribe((theValue: ISymanticLogs) => {
      this.PrepareData(theValue);
    });

    this.myRenderer.RenderSubject().subscribe((theX) => {
      this.RenderTable();
    });

    this.myRenderer.rendererReady.subscribe(() => {
      if (this.myRenderer.Engine == null) {
        return;
      }

      this.myRenderer.Engine?.prepareLogFailed.subscribe(() => {
        this.myProgressGlobal.hideProgress.next();
        this.myMessageService.PostError("Error loading log file, check if parser config and log matches...");
        this.myFile = undefined;
      });

      this.myRenderer.Engine?.appendLogFailed.subscribe(() => {
        this.myProgressGlobal.hideProgress.next();
        this.myMessageService.PostError("Error upload log file...");
        this.myFile = undefined;
      });
    });

    this.mySessionService.sessionReset.subscribe(() => {
      this.myParserConfigEvents.ClearParser();
      this.CleanupTableStuff();
    });

    aTrace.info(`Renderer Started...`);
  }


  private SetupParser(theFile: File) {
    const aFileReader = new FileReader();
    aFileReader.onloadend = (_) => {
      if (aFileReader.result == null) {
        this.myMessageService.PostError("Error reading file...");
        return;
      }

      const aLogDefinition = YAML.parse(aFileReader.result.toString());
      if (!LogDefinition.Validate(aLogDefinition)) {
        this.myMessageService.PostError("Invalid log definition file...");
        return;
      }

      this.myParserConfigEvents.SetCurrentParser(aLogDefinition, "file import");
      this.myDialogService.open(ParserSetupViewComponent, { header: 'Parser Configuration', closable: true, closeOnEscape: true });
    };

    aFileReader.readAsText(theFile);
  }


  private PrepareHeader(theHeader: ILogHeader): void {
    const aTrace = new Tracer(this.myLogger, this.myDomain, "PrepareHeader");
    if (this.myTable == null) {
      aTrace.error("Table Not Ready, Header Negotiation Failed...");
      return;
    }

    this.columnConfig = []
    this.myCurrentHeader = theHeader;
    for (let aIndex = 0; aIndex < theHeader.columnNames.length; aIndex++) {
      this.columnConfig.push({ title: theHeader.columnNames[aIndex], field: theHeader.columnNames[aIndex] });
    }

    this.myTable.setColumns(this.columnConfig);
    TableUtils.HideDataColumns(this.myTable);
    this.myTableEvents.headerChange.next(this.columnConfig);
    this.myTableEvents.columnChanges.next(this.myTable.getColumns());
    if (theHeader.logDefinition) {
      this.myParserConfigEvents.SetCurrentParser(LogDefinition.ReverseConvertCSharpToEnglishMarkers(theHeader.logDefinition), "auto detect");
      aTrace.info("Auto Parser Set");
    }
  }

  private CleanupTableStuff() {
    const aTrace = new Tracer(this.myLogger, this.myDomain, "CleanupTableStuff");
    if (this.myTable == null) {
      aTrace.error("Table Not Ready, Cleanup Failed...");
      return;
    }

    this.tableData = [];
    this.myTable.clearData();
    this.myCurrentFindResults.clear();
    this.myCurrentFindResults.clear();
    this.myCurrentFindResultsOrdered = [];
    this.myCurrentFindIndex = -1;
    this.myPreviousFind = new FindWithWhat();
  }

  private PrepareData(theData: ISymanticLogs): void {
    if (this.myCurrentHeader == null) {
      return;
    }

    this.tableData = this.tableData.concat(theData.logs);
  }

  private RenderTable(): void {
    const aTrace = new Tracer(this.myLogger, this.myDomain, "RenderTable");
    if (this.myTable == null) {
      aTrace.error(`Table Rendered Failed Due To An Impossible TS Issue.`);
      return;
    }

    const aTimer = timer();
    this.myProgressGlobal.updateProgressPercent.next(-1);
    this.myProgressGlobal.updateProgressMessage.next("Rendering table...");
    this.myTable.setData(this.tableData).then(() => {
      this.myProgressGlobal.hideProgress.next();
      this.myTable?.redraw(true);
      aTrace.info(`Table Rendered in ${aTimer.ms}...`);
    });
  }
}
