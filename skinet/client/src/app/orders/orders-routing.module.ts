import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { OrdersModule } from './orders.module';
import { OrdersComponent } from './orders.component';
import { Router, RouterModule, Routes } from '@angular/router';
import { OrderDetailedComponent } from '../order-detailed/order-detailed.component';

const routes: Routes = [
  {path: '', component: OrdersComponent},
  {path: ':id', component: OrderDetailedComponent, data: {breadcrumb: {alias: 'OrderDetailed'}}}
]

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    RouterModule
  ],
  exports: [
    RouterModule
  ]
})

export class OrdersRoutingModule { }
