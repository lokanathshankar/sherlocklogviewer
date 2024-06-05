import { Component } from '@angular/core';
import { DialogService, DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { LogElementType, LogElementTypeHelper, LogSyntaxType } from '../../common/enums';
import { LogElement, LogSyntax } from '../../common/logdefinition';
import { MessageHelper } from '../../services/messagehelper';
import { ObjectUtils } from '../../utils/objectutils';
import { SyntaxIdProvider } from '../FacsAndConsts';

export class LogElementViewModel {
  public logElement: LogElement;
  public disableFormatter: string = "true";
  constructor(theElement: LogElement, theDisableFormatter: boolean) {
    this.logElement = theElement;
    this.changeFormatterState(theDisableFormatter);
  }

  changeFormatterState(theDisableFormatter: boolean) {
    if (theDisableFormatter) {
      this.disableFormatter = "true";
    }
    else {
      this.disableFormatter = "false";
    }
  }
}

@Component({
  selector: 'app-syntax-setup-view',
  templateUrl: './syntax-setup-view.component.html',
  styleUrls: ['./syntax-setup-view.component.css'],
})
export class SyntaxSetupViewComponent {
  public readonly syntax: LogSyntax = new LogSyntax();
  public readonly elements: LogElementViewModel[] = [];
  public readonly logElementTypes = LogElementTypeHelper.GetOptionArray();

  constructor(public ref: DynamicDialogRef, private myDataGetter: DynamicDialogConfig, private myMessenger: MessageHelper) {
    this.syntax = myDataGetter.data.logSyntax;
    for (var aIndex = 0; aIndex < this.syntax.Elements.length; aIndex++) {
      this.elements.push(new LogElementViewModel(this.syntax.Elements[aIndex], this.doINeedToEnableFormatBox(this.syntax.Elements[aIndex].Type)));
    }

    if (this.syntax.Id == null) {
      this.syntax.Id = SyntaxIdProvider.Instance.GetNewId();
    }
  }

  onAddElement(): void {
    const aToAdd = new LogElement();
    this.elements.push(new LogElementViewModel(aToAdd, true));
    this.syntax.Elements.push(aToAdd);
  }

  onRemoveElement(theElement: LogElementViewModel): void {
    ObjectUtils.RemoveArrayElement(this.elements, theElement)
    ObjectUtils.RemoveArrayElement(this.syntax.Elements, theElement.logElement)
  }

  onElementTypeChanged(theEvent: Event, theModel: LogElementViewModel) {
    theModel.changeFormatterState(this.doINeedToEnableFormatBox(theModel.logElement.Type));
  }

  private doINeedToEnableFormatBox(theType: LogElementType) {
    return !(theType === LogElementType.Date || theType === LogElementType.Time || theType === LogElementType.DateTime);
  }

  onCancel(): void {
    this.ref.close(null);
  }

  onConfirm(): void {
    if (!LogSyntax.Validate(this.syntax)) {
      this.myMessenger.PostError("Syntax invalid, please check and update mandatory fields");
      return;
    }

    this.ref.close(this.syntax);
  }
}
