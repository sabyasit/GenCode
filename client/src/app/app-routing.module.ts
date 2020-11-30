import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { OpenapiComponent } from './openapi/openapi.component';
import { ApibuilderComponent } from './apibuilder/apibuilder.component'

const routes: Routes = [
  { path: 'builder', component: ApibuilderComponent },
  { path: 'openapi', component: OpenapiComponent },
  { path: '**', redirectTo: 'openapi' }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
