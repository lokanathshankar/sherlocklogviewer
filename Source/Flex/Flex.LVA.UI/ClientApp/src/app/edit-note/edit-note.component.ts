import { Component } from '@angular/core';
import { DynamicDialogConfig, DynamicDialogRef } from 'primeng/dynamicdialog';
import { MessageHelper } from '../../services/messagehelper';
import { TableRenderer } from '../../services/render.service';


export class Note {
  public logFormatted: string | null = null;
  public contentFormatted: string | null = null;
  public dateTime: string | null = null;
  public shortHandString: string | null = null;
  public range: number[] | null = null;
  public content: string | null = null;
  constructor(theRange: number[] | null) {
    this.range = theRange;
  }

  Refresh(): void { }
}


@Component({
  selector: 'app-edit-note',
  templateUrl: './edit-note.component.html',
  styleUrls: ['./edit-note.component.css']
})
export class EditNoteComponent {
  public readonly currentNote: Note;
  constructor(
    private myTableRenderer: TableRenderer,
    private myDialogRef: DynamicDialogRef,
    private myDataGetter: DynamicDialogConfig,
    private myMessenger: MessageHelper) {
    this.currentNote = myDataGetter.data.note;
    if (!this.currentNote.range?.length) {
      this.currentNote.logFormatted = "No logs were selected to load...";
    }
    else {
      this.myTableRenderer.Engine?.getRawLogs(this.currentNote.range).subscribe(theX => {
        console.log(theX)
        this.currentNote.logFormatted = "<textarea>" + theX + "</textarea>"
        this.currentNote.dateTime = ""
        console.log(this.currentNote.logFormatted)
      });
    }
  }

  onConfirm(): void { this.myDialogRef.close(this.currentNote); }
  onCancel(): void { this.myDialogRef.close(null); }
  onNoteContentChanged(event: any): void {
    this.currentNote.content = event.textValue
  }
}
