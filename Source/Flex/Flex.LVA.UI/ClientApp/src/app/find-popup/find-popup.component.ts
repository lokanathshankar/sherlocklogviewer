import { Component } from '@angular/core';
import { MenuEvents } from '../top-menu/TopMenuEvents';

@Component({
  selector: 'app-find-popup',
  templateUrl: './find-popup.component.html'
})
export class FindPopupComponent {
  public visible: boolean = false;
  
  constructor(private myMenuEvents: MenuEvents) {
    this.myMenuEvents.findClicked.subscribe(() => {
      this.visible = true;
    });
  }
}
