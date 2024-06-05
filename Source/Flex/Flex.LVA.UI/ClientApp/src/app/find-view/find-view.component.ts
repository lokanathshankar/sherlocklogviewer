import { Component } from '@angular/core';
import { FindEvents, FindWith, FindWithWhat } from './FindEvents';

@Component({
  selector: 'app-find-view',
  templateUrl: './find-view.component.html',
  styleUrls: ['./find-view.component.css']
})
export class FindViewComponent {
  public searchText: string = "";
  public searchFilters: FindWith[] = [FindWith.Like, FindWith.Equals, FindWith.Regex];
  public selectedFilter: any = FindWith.Like;
  constructor(private myFindEvents: FindEvents) {
  }

  onFindNext(): void {
    const aFindWith = new FindWithWhat();
    aFindWith.With = this.selectedFilter;
    aFindWith.What = this.searchText;
    this.myFindEvents.findNext.next(aFindWith);
  }

  onFindPrevious(): void {
    const aFindWith = new FindWithWhat();
    aFindWith.With = this.selectedFilter;
    aFindWith.What = this.searchText;
    this.myFindEvents.findPrevious.next(aFindWith);
  }

  onFindAll(): void {
    const aFindWith = new FindWithWhat();
    aFindWith.With = this.selectedFilter;
    aFindWith.What = this.searchText;
    this.myFindEvents.findAll.next(aFindWith);
  }

  onClearAll(): void {
    this.myFindEvents.clearAll.next();
  }
}
