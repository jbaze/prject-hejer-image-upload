import { Component, OnInit } from '@angular/core';
import { Router, RouterOutlet } from '@angular/router';
import { TranslateAppService } from './services/translate.service';
import { HeaderComponent } from './components/header/header.component';
import { AuthService } from './services/auth.service';
import { User } from './models/auth.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, HeaderComponent, CommonModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent implements OnInit {
  title = 'angular-app-template';
  currentUser: User | null = null;

  constructor(
    private _translateService: TranslateAppService,
    private _auth: AuthService
  ){
      this._translateService.setTranslateDefault();
  }

  ngOnInit(): void {
    this._auth.currentUserSubject.subscribe({
      next: (user) => {
        this.currentUser = user;
      }
    });
  }
}
