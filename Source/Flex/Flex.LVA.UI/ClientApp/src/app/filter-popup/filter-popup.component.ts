import { Component } from '@angular/core';
import { MenuEvents } from '../top-menu/TopMenuEvents';

@Component({
  selector: 'app-filter-popup',
  templateUrl: './filter-popup.component.html',
})
export class FilterPopupComponent {
  public visible: boolean = false;
  constructor(private myMenuEvents: MenuEvents) {
    this.myMenuEvents.filterClicked.subscribe(() => {
      this.visible = true;
    });
  }
}
