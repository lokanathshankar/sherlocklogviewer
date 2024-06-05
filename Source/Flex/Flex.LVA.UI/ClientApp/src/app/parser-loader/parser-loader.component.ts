import { Component } from '@angular/core';
import { LogElementType, LogElementTypeHelper } from '../../common/enums';
import { LogDefinition } from '../../common/logdefinition';
import { UserWorkboxService } from '../../services/userworkbox.service';
import { ParserEventsAndUpdates } from '../parser-setup-view/ParserEvents';

@Component({
  selector: 'app-parser-loader',
  templateUrl: './parser-loader.component.html',
  styleUrls: ['./parser-loader.component.css']
})
export class ParserLoaderComponent {
  public logDefinitions: LogDefinition[] = [];
  constructor(
    private myParserEvents  :ParserEventsAndUpdates,
    private myUserWorkBox: UserWorkboxService) {
    const aPatterns = myUserWorkBox.GetKnownPatterns();
    if (aPatterns) {
      this.logDefinitions = aPatterns;
    }
  }

  getStringFromType(theElement: LogElementType) {
    return LogElementTypeHelper.getStringFromType(theElement);
  }

  onLoadParser(theDefToLoad: LogDefinition): void {
    this.myParserEvents.SetCurrentParser(theDefToLoad, "stored");
  }
}
