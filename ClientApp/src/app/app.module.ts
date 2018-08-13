import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './app.component';
import { AlertComponent } from './_directives/alert.component'; 
import { UploadsComponent } from './uploads/uploads.component';
import { RegisterComponent } from './register/register.component';
import { LoginComponent } from './login/login.component';

import { AlertService } from './_services/alert.service';
import { UserService } from './_services/user.service';
import { UploadService } from './_services/upload.service';
import { AuthenticationService } from './_services/authentication.service';
import { AuthGuard } from './_guards/auth.guard';
import { JwtInterceptor } from './_helpers/jwt.interceptor';
@NgModule({
  declarations: [
    AppComponent,
    AlertComponent,
    RegisterComponent,
    LoginComponent,
    UploadsComponent
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    HttpClientModule,
    FormsModule,
    RouterModule.forRoot([
      { path: '', component: UploadsComponent, pathMatch: 'full', canActivate: [AuthGuard]},
      { path: 'users/register', component: RegisterComponent },
      { path: 'users/login', component: LoginComponent },
    ])
  ],
  providers: [
    AlertService,
    AuthenticationService,
    UploadService,
    UserService, 
    {
      provide: HTTP_INTERCEPTORS,
      useClass: JwtInterceptor,
      multi: true
    },
    AuthGuard],
  bootstrap: [AppComponent]
})
export class AppModule { }
