import { Component } from '@angular/core';
import { MenuEvents } from '../top-menu/TopMenuEvents';

@Component({
  selector: 'app-column-options-popup',
  templateUrl: './column-options-popup.component.html',
})
export class ColumnOptionsPopupComponent {
  public visible: boolean = false;

  constructor(private myMenuEvents: MenuEvents) {
    this.myMenuEvents.columnOptionsClicked.subscribe(() => {
      this.visible = true;
    });
  }
}
