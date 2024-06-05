import { LogElementType } from "./enums";
import { LogDefinition } from "./logdefinition";

export interface ILogHeader {
  columnNames: Array<string>;
  columnTypes: Array<LogElementType>;
  logDefinition: LogDefinition;
}

export interface ISymanticLogs {
  logs: any[];
}

