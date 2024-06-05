import { Injectable } from "@angular/core";
import { Subject } from "rxjs";
import { ColumnComponent, ColumnDefinition, RowComponent, TabulatorFull } from "tabulator-tables";
import { ITableDataProvider } from "../../interfaces/TableInterfaces";
import { ColumnEvents } from "../column-options/ColumnEvents";

@Injectable({
  providedIn: 'root'
})
export class TableEvents {
  public tableRowHovered: Subject<number> = new Subject();
  public navigateToRow: Subject<number> = new Subject();
  public handleBookmark: Subject<RowComponent> = new Subject();
  public findResults: Subject<RowComponent[]> = new Subject();
  public headerChange: Subject<any[]> = new Subject();
  public columnChanges: Subject<ColumnComponent[]> = new Subject();
  public headerRearranged: Subject<ColumnDefinition[]> = new Subject();
}

export class TableColumns {
  public static ID = 'LogId';
  public static Header = 'Header';
}

export class TableColumnUtil {
  constructor(private myTable: TabulatorFull, private myColumnEvents: ColumnEvents, private myTableEvents: TableEvents | null = null) {
    myColumnEvents.showColumn.subscribe((theX: string) => {
      if (this.myTable.getColumn(theX).isVisible()) {
        return;
      }

      this.myTable.showColumn(theX);
    });

    myColumnEvents.hideColumn.subscribe((theX: string) => {
      if (!this.myTable.getColumn(theX).isVisible()) {
        return;
      }

      this.myTable.hideColumn(theX);
    });

    myColumnEvents.pinColumn.subscribe((theX: string) => {
      if (this.myTable.getColumn(theX).getDefinition().frozen) {
        return;
      }

      this.myTable.getColumn(theX).updateDefinition({ title: theX, frozen: true }).then(() => {
        this.myTableEvents?.columnChanges.next(this.myTable.getColumns());
      });
    });

    myColumnEvents.unpinColumn.subscribe((theX: string) => {
      if (!this.myTable.getColumn(theX).getDefinition().frozen) {
        return;
      }

      this.myTable.getColumn(theX).updateDefinition({ title: theX, frozen: false }).then(() => {
        this.myTableEvents?.columnChanges.next(this.myTable.getColumns());
      });;
    });
  }

  Hold(): void {
  }
}

export class TableUtils {
  public static HideDataColumns(theTable: TabulatorFull | undefined): void {
    if (theTable == null) {
      return;
    }

    theTable.hideColumn(TableColumns.Header);
    theTable.hideColumn(TableColumns.ID);
  }
}

export class TableDataProvider implements ITableDataProvider {
  constructor(private myTable: TabulatorFull) {
  }

  ClearFilters(): void {
    this.myTable.clearFilter(true);
  }
  ApplyFilter(theFilters: any): void {
    this.myTable.setFilter(theFilters);
  }

  GetAllRowIds(): number[] {
    let aRows: number[] = [];
    this.myTable.getRows().forEach((theRow: RowComponent) => {
      aRows.push(theRow.getIndex());
    });

    return aRows;
  }

  GetSelectedRowIds(): number[] {
    let aRows: number[] = [];
    this.GetSelectedRows().forEach((theRow: RowComponent) => {
      aRows.push(theRow.getIndex());
    });

    return aRows;
  }

  GetSelectedRows(): RowComponent[] {
    return this.myTable.getSelectedRows();
  }

  GetRows(theRowId: number[]): RowComponent[] {
    let aRows: RowComponent[] = [];
    this.myTable.getRow
    theRowId.forEach((theId) => {
      aRows.push(this.myTable.getRow(theId));
    });

    return aRows;
  }
}
