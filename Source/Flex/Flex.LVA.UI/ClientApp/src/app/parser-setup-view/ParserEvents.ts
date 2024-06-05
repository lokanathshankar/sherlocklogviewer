import { Injectable } from "@angular/core";
import { Subject } from "rxjs";
import { LogDefinition } from "../../common/logdefinition";
import { MessageHelper } from "../../services/messagehelper";

@Injectable({
  providedIn: 'root'
})
export class ParserEventsAndUpdates {
  public parserConfigSet: Subject<LogDefinition> = new Subject();
  public parserConfigChanged: Subject<LogDefinition> = new Subject();
  private myDefinition: LogDefinition | null = null;

  constructor(private myMessageService: MessageHelper) {
    
  }
  public SetCurrentParser(theNewDef: LogDefinition, theReason : string): void {
    const aSet = this.myDefinition == null;
    this.myDefinition = theNewDef;
    if (!aSet) {
      this.parserConfigSet.next(theNewDef);
    }
    else {
      this.parserConfigChanged.next(theNewDef);
    }

    console.log(`Parser Set To ${JSON.stringify(theNewDef)} for reason '${theReason}'`);
    this.myMessageService.PostSuccess(`Parser loaded from a ${theReason} configuration`);
  }

  public ClearParser(): void {
    this.myDefinition = null;
  }

  public GetCurrentParser(): LogDefinition | null {
    return this.myDefinition;
  }
}
