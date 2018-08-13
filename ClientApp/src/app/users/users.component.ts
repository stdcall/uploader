import { Component } from '@angular/core';
import { User } from '../_models/user';
import { UserService } from '../_services/user.service';

@Component({
    selector: 'users',
    templateUrl: './users.component.html'
  })
  export class UsersComponent {
    public users: User[];
    currentUser: User;
    constructor(private userService: UserService) {
      this.currentUser = JSON.parse(localStorage.getItem('currentUser'));
    }
  }