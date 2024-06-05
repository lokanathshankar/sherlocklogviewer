import { RowComponent } from "tabulator-tables";

export interface ITableDataProvider {
  GetSelectedRows(): Array<RowComponent>;
  GetAllRowIds(): Array<number>;
  GetSelectedRowIds(): Array<number>;
  ClearFilters(): void;
  ApplyFilter(theFilters: any): void;
}
