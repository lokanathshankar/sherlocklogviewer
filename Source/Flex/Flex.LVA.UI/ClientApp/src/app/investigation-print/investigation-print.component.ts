import { Component } from '@angular/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { Note } from '../edit-note/edit-note.component';

@Component({
  selector: 'app-investigation-print',
  templateUrl: './investigation-print.component.html',
  styleUrls: ['./investigation-print.component.css']
})
export class InvestigationPrintComponent {
  public printData: any = "";
  private myNotesToPrint: Note[];
  constructor(
    public myDialogRef: DynamicDialogRef, private myDataGetter: DynamicDialogConfig) {
    this.myNotesToPrint = myDataGetter.data.notes;
    this.myNotesToPrint.forEach((theX: Note) => {
      this.printData += `<p><strong>${theX.shortHandString}</strong></p>`
      if (theX?.contentFormatted) {
        this.printData += `<p>${theX.contentFormatted}</p>`
      }
      else {
        this.printData += `<p>Not provided</p>`
      }

      this.printData += `</br>`
      if (theX?.logFormatted) {
        this.printData += `<p>${theX.logFormatted}</p>`
      }
      else {
        this.printData += `<p>Not relevant logs</p>`
      }

      this.printData += `</br>`
    });

    console.log(this.printData);
  }

  public onCancel(): void {
    this.myDialogRef.close()
  }

  public onCopyToClipBoard(): void {
    navigator.clipboard.writeText(this.printData);
  }

  public onPrintNote(): void {
    const mywindow = window.open(
      "",
      "PRINT");
    if (!mywindow) {
      return;
    }

    mywindow.document.write(this.printData);
    mywindow.print();
    mywindow.close();
  }
}
