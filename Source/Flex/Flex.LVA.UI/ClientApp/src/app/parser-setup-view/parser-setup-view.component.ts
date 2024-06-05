import { Component } from '@angular/core';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { LogElementType, LogElementTypeHelper } from '../../common/enums';
import { LogDefinition, LogSyntax } from '../../common/logdefinition';
import { MessageHelper } from '../../services/messagehelper';
import { IUserWorkboxService, UserWorkboxService } from '../../services/userworkbox.service';
import { SyntaxSetupViewComponent } from '../syntax-setup-view/syntax-setup-view.component';
import { TopMenuComponent } from '../top-menu/top-menu.component';
import { MenuEvents } from '../top-menu/TopMenuEvents';
import { ParserEventsAndUpdates } from './ParserEvents';

@Component({
  selector: 'app-parser-setup-view',
  templateUrl: './parser-setup-view.component.html',
  styleUrls: ['./parser-setup-view.component.css'],
})
export class ParserSetupViewComponent {
  public logDefinition: LogDefinition = new LogDefinition();
  private ref?: DynamicDialogRef;
  constructor(
    private myDialogService: DialogService,
    private myDialogRef: DynamicDialogRef,
    private myParserEvents: ParserEventsAndUpdates,
    private myTopMenuEvents: MenuEvents,
    private myUserWorkBoxService: UserWorkboxService,
    private myMessenger: MessageHelper) {
    const aConfig = myParserEvents.GetCurrentParser();
    if (aConfig == null) {
      return;
    }

    this.logDefinition = aConfig;
  }

  getStringFromType(theElement: LogElementType) {
    return LogElementTypeHelper.getStringFromType(theElement);
  }

  onConfirmParser(): void {
    if (!LogDefinition.Validate(this.logDefinition)) {
      this.myMessenger.PostError("Invalid log definition file...");
      return;
    }

    this.myParserEvents.SetCurrentParser(this.logDefinition, "custom");
    this.myUserWorkBoxService.AddToKnownPattern(this.logDefinition);
    this.myDialogRef.close();
  }

  onCancelParser(): void {
    this.logDefinition = new LogDefinition();
    this.myDialogRef.close();
  }

  onAddSyntax(): void {
    this.ref = this.myDialogService.open(SyntaxSetupViewComponent, { data: { logSyntax: new LogSyntax() }, header: 'Create Syntax', closable: true, closeOnEscape: true });
    this.ref.onClose.subscribe((theX: LogSyntax | null) => {
      if (theX == null) {
        return;
      }

      this.logDefinition.Syntaxes.push(theX);
    });
  }

  onEditSyntax(theLogSyntax: LogSyntax): void {
    this.ref = this.myDialogService.open(SyntaxSetupViewComponent, { data: { logSyntax: theLogSyntax }, header: 'Edit Syntax', closable: true, closeOnEscape: true });
  }

  onRemoveSyntax(theLogSyntax: LogSyntax): void {
    const aIndexOfSyntax = this.logDefinition.Syntaxes.indexOf(theLogSyntax);
    if (aIndexOfSyntax == -1) {
      return;
    }

    this.logDefinition.Syntaxes.splice(aIndexOfSyntax, 1);
  }
}
