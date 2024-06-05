import { Component } from '@angular/core';
import { ColumnComponent, ColumnDefinition } from 'tabulator-tables';
import { UiSessionService } from '../../services/uisession.service';
import { SharedDataFactory } from '../FacsAndConsts';
import { TableEvents } from '../table-view/TableEvents';
import { ColumnEvents, ColumnSelection } from './ColumnEvents';

@Component({
  selector: 'app-column-options',
  templateUrl: './column-options.component.html',
  styleUrls: ['./column-options.component.css']
})
export class ColumnOptionsComponent {
  private readonly myDataContextShared = SharedDataFactory.Instance.getSharedDataContextRows("ColumnOptionsComponent");
  public readonly columnSelections: ColumnSelection[] = this.myDataContextShared.rows;
  private myColumnMap: Map<string, ColumnSelection> = new Map();

  constructor(
    private myTableEvents: TableEvents,
    private mySessionService: UiSessionService,
    private myColumnEvents: ColumnEvents) {
    myTableEvents.columnChanges.subscribe((theX: ColumnComponent[]) => {
      this.myDataContextShared.ClearRows();
      theX.forEach((theX) => {
        let aColumn: ColumnSelection | undefined;
        if (this.myColumnMap.has(theX.getField())) {
          aColumn = this.myColumnMap.get(theX.getField());
        }
        else {
          aColumn = new ColumnSelection();

          this.myColumnMap.set(theX.getField(), aColumn);
        }

        if (aColumn == null) {
          return
        }

        aColumn.value = theX.getField();
        aColumn.visible = theX.isVisible();
        aColumn.pinned = theX.getDefinition().frozen;
        this.columnSelections.push(aColumn);
      });
    });

    this.mySessionService.sessionReset.subscribe(() => {
      this.myDataContextShared.ClearRows();
    });
  }

  onVisibilityChanged(value: Event): void {
    // TODO : Some sort of change detections?
    this.columnSelections.forEach((theX) => {
      if (theX.visible) {
        this.myColumnEvents.showColumn.next(theX.value);
      }
      else {
        this.myColumnEvents.hideColumn.next(theX.value);
      }
    });
  }

  onPinChanged(value: Event) {
    this.columnSelections.forEach((theX) => {
      if (theX.pinned) {
        this.myColumnEvents.pinColumn.next(theX.value);
      }
      else {
        this.myColumnEvents.unpinColumn.next(theX.value);
      }
    });
  }
}
