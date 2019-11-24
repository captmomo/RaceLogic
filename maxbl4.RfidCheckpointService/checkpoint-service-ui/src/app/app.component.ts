import {ChangeDetectorRef, Component, OnDestroy} from '@angular/core';
import {MediaMatcher} from "@angular/cdk/layout";

@Component({
  selector: 'app-root',
  template: `      
      <mat-toolbar color="primary" class="fixed-top navbar navbar-dark">
          <span class="d-flex flex-grow-1">
              <a class="navbar-brand" routerLink="">Checkpoint Service</a>
          </span>
          <button mat-icon-button (click)="sidenav.toggle()"
                  [hidden]="!mobileQuery.matches"><i class="material-icons">menu</i></button>
      </mat-toolbar>
    <mat-sidenav-container class="h-100">
        <mat-sidenav #sidenav [mode]="mobileQuery.matches ? 'over' : 'side'" [opened]="!mobileQuery.matches" position="end" 
                     [fixedInViewport]="mobileQuery.matches"
                        [fixedTopGap]="mobileQuery.matches ? 56 : 64"
                     (click)="mobileQuery.matches ? sidenav.toggle() : false">
            <mat-nav-list>
                <a mat-list-item routerLink="">Dashboard</a>
                <a mat-list-item routerLink="options">Options</a>
                <a mat-list-item routerLink="monitor">Monitor</a>
                <a mat-list-item routerLink="logs">Logs</a>
            </mat-nav-list>
        </mat-sidenav>

        <mat-sidenav-content>
            <div class="container-fluid">
                <router-outlet></router-outlet>
            </div>
        </mat-sidenav-content>
    </mat-sidenav-container>
  `,
  styles: []
})
export class AppComponent implements OnDestroy {
  public mobileQuery: MediaQueryList;
  private readonly _mobileQueryListener: () => void;

  constructor(changeDetectorRef: ChangeDetectorRef, media: MediaMatcher) {
    this.mobileQuery = media.matchMedia('(max-width: 600px)');
    this._mobileQueryListener = () => console.log("detected");
    this.mobileQuery.addListener(this._mobileQueryListener);
  }

  ngOnDestroy(): void {
    this.mobileQuery.removeListener(this._mobileQueryListener);
  }
}