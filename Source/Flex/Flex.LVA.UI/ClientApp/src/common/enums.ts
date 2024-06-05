export enum LogElementType {
  Unknown = 0,
  String = 1,
  Number = 2,
  DateTime = 3,
  Time = 4,
  Date = 5,
  Bool = 6,
}

export enum LogSyntaxType {
  Parent = 1,
  Child = 2,
}

export enum LogDataType {
  PlainText = 1,
  Xml = 2,
  Json = 3,
  Evtx = 4,
  Auto = 5,
  Delimited = 6
}

export class LogElementTypeHelper {
  public static GetOptionArray(): any {
    return [
      { label: "Ignore", value: LogElementType.Unknown },
      { label: "String", value: LogElementType.String },
      { label: "Number", value: LogElementType.Number },
      { label: "Date Only", value: LogElementType.Date },
      { label: "Time Only", value: LogElementType.Time },
      { label: "Date and Time", value: LogElementType.DateTime },
      { label: "True or False", value: LogElementType.Bool }];
  }

  public static getStringFromType(theElement: LogElementType) {
    // @ts-ignore
    return LogElementTypeHelper.GetOptionArray().find(theX => theX.value == theElement).label;
  }
}
