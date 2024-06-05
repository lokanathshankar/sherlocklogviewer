import { LogDataType, LogElementType, LogSyntaxType } from "./enums";
export class LogElement {
  Name: string = "";
  EndSeparator: string | null = null;
  Type: LogElementType = LogElementType.String;
  ChildSyntaxId: number | null = null
  DateTimeFormat : string | null = null
  static AreSame(theDef1: LogElement, theDef2: LogElement): boolean {
    if (theDef1.Name != theDef2.Name) {
      return false
    }

    if (theDef1.DateTimeFormat != theDef2.DateTimeFormat) {
      return false
    }

    if (theDef1.EndSeparator != theDef2.EndSeparator) {
      return false
    }

    if (theDef1.Type != theDef2.Type) {
      return false
    }

    if (theDef1.ChildSyntaxId != theDef2.ChildSyntaxId) {
      return false
    }

    return true;
  }
}

export class LogSyntax {
  Id: number | null = null;
  BeginMarker: string | null = null;
  EndMarker: string | null = null;
  SyntaxType: LogSyntaxType = LogSyntaxType.Parent;
  Elements: Array<LogElement> = [];

  static Validate(theSyntax: LogSyntax): boolean {
    if (theSyntax.Id == null || theSyntax.Id < 0) {
      return false;
    }

    if (theSyntax.EndMarker == null) {
      return false;
    }

    return true;
  }

  static AreSame(theDef1: LogSyntax, theDef2: LogSyntax): boolean {
    if (theDef1.BeginMarker != theDef2.BeginMarker) {
      return false
    }

    if (theDef1.EndMarker != theDef2.EndMarker) {
      return false
    }

    if (theDef1.SyntaxType != theDef2.SyntaxType) {
      return false
    }

    if (theDef1.Elements.length != theDef2.Elements.length) {
      return false;
    }

    for (var i = 0; i < theDef1.Elements.length; i++) {
      if (!LogElement.AreSame(theDef1.Elements[i], theDef2.Elements[i])) {
        return false;
      }
    }

    return true;
  }
}

export class LogDefinition {
  Name: string | null = null;
  HeaderLineCount: number | null = null;
  Syntaxes: Array<LogSyntax> = [];
  LogFileType: LogDataType = LogDataType.Auto;
  AutoDetected: boolean = false;
  static ReverseConvertCSharpToEnglishMarkers(theLogDef: LogDefinition): LogDefinition {
    for (var aItem of theLogDef.Syntaxes) {
      if (aItem.EndMarker === '\n') {
        aItem.EndMarker = 'newline';
      }

      if (aItem.BeginMarker === '\n') {
        aItem.BeginMarker = 'newline';
      }
    }

    for (var aELement of theLogDef.Syntaxes.flatMap(a => a.Elements)) {
      if (!aELement) {
        continue;
      }

      if (aELement.EndSeparator === '\t') {
        aELement.EndSeparator = "tab"
      }

      if (aELement.EndSeparator === '\t') {
        aELement.EndSeparator = "tab"
      }
    }

    return theLogDef;
  }

  static Validate(theFileContents: LogDefinition): boolean {
    let aReturn = true;
    theFileContents.Syntaxes.forEach((theX) => {
      if (!LogSyntax.Validate(theX)) {
        aReturn = false;
      }
    });

    return aReturn;
  }

  static AreSame(theDef1: LogDefinition, theDef2: LogDefinition): boolean {
    if (theDef1.HeaderLineCount != theDef2.HeaderLineCount) {
      return false
    }

    if (theDef1.Syntaxes.length != theDef2.Syntaxes.length) {
      return false;
    }

    for (var i = 0; i < theDef1.Syntaxes.length; i++) {
      if (!LogSyntax.AreSame(theDef1.Syntaxes[i], theDef2.Syntaxes[i])) {
        return false;
      }
    }

    return true;
  }
}
