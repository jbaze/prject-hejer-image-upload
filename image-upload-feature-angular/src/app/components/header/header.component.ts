import { Component, OnInit } from '@angular/core';
import { AuthService } from '../../services/auth.service';
import { User } from '../../models/auth.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-header',
  imports: [CommonModule],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent implements OnInit {

  currentUser: User | null = null;

  constructor(
    private _auth: AuthService
  ){}

  ngOnInit(): void {
    this._auth.currentUserSubject.subscribe({
      next: (user) => {
        if (user) {
          this.currentUser = user;
        }
      }
    });
  }

  logOut() {
    this._auth.logout();
  }
 }
