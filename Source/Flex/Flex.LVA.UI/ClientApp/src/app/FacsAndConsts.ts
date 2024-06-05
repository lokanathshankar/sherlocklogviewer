import { LogDefinition } from "../common/logdefinition";
import { ITableDataProvider } from "../interfaces/TableInterfaces";

export class MessagingConstants {
  public static GlobalToast = 'GlobalToast';
}

export class TableConstants {
  public static MainTable = 'MainTable';
  public static BookmarkTable = 'BookmarkTable';
  public static FindResultTable = 'FindResultTable';
}

interface ISharedDataContextRows {
  readonly Id: string;
  readonly rows: any[];
  ClearRows(): void;
  RemoveRow(theFilterToRemove: any): void;
}

class SharedDataContextRows implements ISharedDataContextRows {
  ClearRows() {
    this.rows.length = 0;
  }

  RemoveRow(theFilterToRemove: any) {
    const aIndex = this.rows.indexOf(theFilterToRemove);
    if (aIndex != -1) {
      this.rows.splice(aIndex, 1);
    }
  }

  AddRow(theFilterToAdd: any) {
    const aIndex = this.rows.push(theFilterToAdd);
  }

  public constructor(theId: string) {
    this.Id = theId;
    this.rows = [];
  }

  readonly rows: any[];
  readonly Id: string;
}



export class SharedDataFactory {
  private myDataCache: Map<string, ISharedDataContextRows> = new Map();
  private static _instance: SharedDataFactory;

  private constructor() {
    //...
  }

  getSharedDataContextRows(theViewId: string): ISharedDataContextRows {
    if (!this.myDataCache.has(theViewId)) {
      this.myDataCache.set(theViewId, new SharedDataContextRows(theViewId));
    }

    //@ts-ignore
    return this.myDataCache.get(theViewId);
  }

  public static get Instance() {
    return this._instance || (this._instance = new this());
  }
}

export class SyntaxIdProvider {
  private static _instance: SyntaxIdProvider;
  private myIdCounter: number = 0;
  public GetNewId(): number {
    this.myIdCounter = this.myIdCounter + 1;
    return this.myIdCounter;
  }

  public static get Instance() {
    return this._instance || (this._instance = new this());
  }
}


export class TableFactory {
  private static _instance: TableFactory;
  private myTableCache: Map<string, ITableDataProvider> = new Map();
  public GetMainTable(): ITableDataProvider | undefined {
    return this.myTableCache.get(TableConstants.MainTable);
  }

  public GetBookmarkTable() : ITableDataProvider | undefined  {
    return this.myTableCache.get(TableConstants.BookmarkTable);
  }

  public GetFindTable (): ITableDataProvider | undefined  {
    return this.myTableCache.get(TableConstants.FindResultTable);
  }

  public RegisterTable(theTableName : string,theTable: ITableDataProvider): void {
    this.myTableCache.set(theTableName, theTable);
  }

  public static get Instance() {
    return this._instance || (this._instance = new this());
  }
}
