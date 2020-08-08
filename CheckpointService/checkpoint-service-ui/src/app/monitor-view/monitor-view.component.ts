import {Component, OnDestroy, OnInit} from '@angular/core';
import {GridOptions, GridApi, ColumnApi} from 'ag-grid-community';
import {CheckpointService} from '../service/checkpoint.service';
import {Observable, Subscription} from 'rxjs';
import {Checkpoint} from '../model/checkpoint';
import { formatDate } from '../util/formatters';

@Component({
  selector: 'app-monitor-view',
  template: `
      <div class="row align-items-center">
          <div class="col-auto">
              <mat-radio-group [(ngModel)]="display">
                  <mat-radio-button [value]="displayType.regular">Regular</mat-radio-button>
                  <mat-radio-button class="ml-1" [value]="displayType.aggregated">Aggregated</mat-radio-button>
                  <mat-radio-button class="ml-1" [value]="displayType.lowRps">Low RPS</mat-radio-button>
                  <mat-radio-button class="ml-1" [value]="displayType.all">All</mat-radio-button>
              </mat-radio-group>
          </div>
          <div class="col">
              <mat-form-field class="w-100">
                  <input matInput placeholder="Filter" #tagFilter>
              </mat-form-field>
          </div>
      </div>
      <div class="row flex-grow-1 flex-column">
          <ag-grid-angular
                  class="ag-theme-balham h-100"
                  [gridOptions]="gridOptions"
                  [quickFilterText]="tagFilter.value">
          </ag-grid-angular>
      </div>
  `,
  host: {class: 'flex-container'},
  styles: []
})
export class MonitorViewComponent implements OnInit, OnDestroy {
  displayType = DisplayType;
  gridOptions: GridOptions = {
    columnDefs: [
      {headerName: 'Seq', field: 'id', width: 50, sort: 'desc', getQuickFilterText: () => ''},
      {headerName: 'Time', field: 'timestamp', width: 80, valueFormatter: v => formatDate(v.value), getQuickFilterText: () => ''},
      {headerName: 'RiderId', field: 'riderId'},
      {headerName: 'Count', field: 'count', width: 60, getQuickFilterText: () => ''},
      {headerName: 'Rps', field: 'rps', width: 60, valueFormatter: p => p.value.toFixed(1), getQuickFilterText: () => ''},
      {headerName: 'Aggregated', field: 'aggregated', width: 60, getQuickFilterText: () => ''},
    ],
    defaultColDef: {
      sortable: true,
      resizable: true
    },
    onGridSizeChanged: params => params.api.sizeColumnsToFit(),
    onGridReady: params => {
      this.api = params.api;
      this.columnApi = params.columnApi;
      this.display = DisplayType.regular;
    }
  };


  constructor(private checkpointService: CheckpointService) {}

  private subscription: Subscription;
  private _display: DisplayType;
  private api: GridApi;
  private columnApi: ColumnApi;
  get display(): DisplayType {
    return this._display;
  }

  set display(value: DisplayType) {
    this._display = value;
    let $checkpoints: Observable<Checkpoint[]>;
    switch (value) {
      case DisplayType.all:
        this.columnApi.setColumnsVisible(['count', 'rps', 'aggregated'], true);
        $checkpoints = this.checkpointService.$checkpoints;
        break;
      case DisplayType.regular:
        this.columnApi.setColumnsVisible(['count', 'rps', 'aggregated'], false);
        $checkpoints = this.checkpointService.$regularCheckpoints;
        break;
      case DisplayType.aggregated:
        this.columnApi.setColumnsVisible(['aggregated'], false);
        this.columnApi.setColumnsVisible(['count', 'rps'], true);
        $checkpoints = this.checkpointService.$aggregatedCheckpoints;
        break;
      case DisplayType.lowRps:
        this.columnApi.setColumnsVisible(['count', 'rps'], true);
        this.columnApi.setColumnsVisible(['aggregated'], false);
        $checkpoints = this.checkpointService.$lowRpscheckpoints;
        break;
    }
    this.subscribeToData($checkpoints);
  }

  subscribeToData($data: Observable<Checkpoint[]>) {
    if (this.subscription) { this.subscription.unsubscribe(); }
    this.api.setRowData(null);
    this.subscription = $data.subscribe(cps => {
      if (cps.length > 0) { this.api.batchUpdateRowData({add: cps}, () => this.autoSizeColumns()); }
    });
  }

  autoSizeColumns() {
    this.columnApi.autoSizeAllColumns();
  }

  ngOnInit() {
  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
}

enum DisplayType {
  regular,
  aggregated,
  lowRps,
  all,
}
