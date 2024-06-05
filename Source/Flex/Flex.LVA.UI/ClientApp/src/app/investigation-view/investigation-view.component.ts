import { Component, OnInit } from '@angular/core';
import { ConfirmationService, MenuItem } from 'primeng/api';
import { DialogService, DynamicDialogRef } from 'primeng/dynamicdialog';
import { RowComponent } from 'tabulator-tables';
import { Logger } from '../../services/logger.service';
import { UiSessionService } from '../../services/uisession.service';
import { Tracer } from '../../utils/tracer';
import { EditNoteComponent, Note } from '../edit-note/edit-note.component';
import { TableFactory } from '../FacsAndConsts';
import { InvestigationPrintComponent } from '../investigation-print/investigation-print.component';
import { MenuEvents } from '../top-menu/TopMenuEvents';
import { InvestigationEvents } from './InvestigationEvents';

@Component({
  selector: 'app-investigation-view',
  templateUrl: './investigation-view.component.html',
  styleUrls: ['./investigation-view.component.css'],
  providers: [ConfirmationService]
})
export class InvestigationViewComponent {
  private myDomain: string = "InvestigationViewComponent";
  public notes: Note[] = []
  public readonly menuItems: MenuItem[] = [];
  private myTempRef?: DynamicDialogRef;

  constructor(
    private myLogger: Logger,
    private myConService: ConfirmationService,
    private myDialogService: DialogService,
    private myMenuEvents: MenuEvents,
    private mySessionService: UiSessionService,
    private myInvEvents: InvestigationEvents) {
    myInvEvents.noteFromBookmarks.subscribe(() => {
      const aTrace = new Tracer(this.myLogger, this.myDomain, `noteFromBookmarks`);
      const aTable = TableFactory.Instance.GetBookmarkTable();
      if (aTable == null) {
        aTrace.debug("Table Is Null, Not Proceeding");
        return;
      }

      this.onCreateNote(aTable.GetAllRowIds());
    });

    myInvEvents.noteFromFindResults.subscribe(() => {
      const aTrace = new Tracer(this.myLogger, this.myDomain, `noteFromFindResults`);
      const aTable = TableFactory.Instance.GetFindTable();
      if (aTable == null) {
        aTrace.debug("Table Is Null, Not Proceeding");
        return;
      }

      this.onCreateNote(aTable.GetAllRowIds());
    });

    myInvEvents.noteFromSelection.subscribe(() => {
      const aTrace = new Tracer(this.myLogger, this.myDomain, `noteFromFindResults`);
      const aTable = TableFactory.Instance.GetMainTable();
      if (aTable == null) {
        aTrace.debug("Table Is Null, Not Proceeding");
        return;
      }

      this.onCreateNote(aTable.GetSelectedRowIds());
    });

    this.mySessionService.sessionReset.subscribe(() => {
      this.notes = []
    });

    this.menuItems = [
      {
        label: 'Investigation',
        icon: 'pi pi-fw pi-book',
        items: [
          {
            label: 'From Slection...',
            icon: 'pi pi-fw pi-check-circle',
            command: () => this.myInvEvents.noteFromSelection.next(),
          },
          {
            label: 'From Bookmarks...',
            icon: 'pi pi-fw pi-bookmark',
            command: () => this.myInvEvents.noteFromBookmarks.next(),
          },
          {
            label: 'From Find Results...',
            icon: 'pi pi-fw pi-search',
            command: () => this.myInvEvents.noteFromFindResults.next(),
          },
        ]
      }
    ];
  }

  onCreateNote(theLogIds: number[]): void {
    const aNote = new Note(theLogIds);
    const aTempRef = this.myDialogService.open(EditNoteComponent, { data: { note: aNote }, header: 'Investigation Note', maximizable: true, contentStyle: { overflow: 'auto' } });
    aTempRef.onClose.subscribe((theX: Note | null) => {
      if (theX == null) {
        return;
      }

      this.notes = this.notes.concat(theX);
    });

  }

  onEditNote(theNote: Note): void {
    this.myDialogService.open(EditNoteComponent, { data: { note: theNote }, header: 'Investigation Note', maximizable: true, contentStyle: { overflow: 'auto' } });
  }

  onRemoveNote(theNote: Note): void {
    const index = this.notes.indexOf(theNote, 0);
    if (index > -1) {
      this.notes.splice(index, 1);
    }
  }

  onPrintNotes(): void {
    this.myDialogService.open(InvestigationPrintComponent, { data: { notes: this.notes }, header: 'Print Investigation', maximizable: true, contentStyle: { overflow: 'auto' } });
  }

  onShowHelp(): void { }

  onDeleteAll(event: Event): void {
    this.myConService.confirm({
      //@ts-ignore
      target: event.target,
      message: 'Are you sure that you want to delete all the note?',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.notes = []
      },
      reject: () => {
        /// nop
      }
    });
  }
}
