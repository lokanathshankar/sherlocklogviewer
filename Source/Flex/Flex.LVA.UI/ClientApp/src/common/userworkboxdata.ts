import { LogSyntaxType } from "./enums";
import { LogDefinition, LogElement } from "./logdefinition";

export class UserWorkBoxData {
  Version: number = 0;
  SavedPatterns: Array<LogDefinition> = []
}
