import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../environments/environment';

interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T[];
  errors: any;
  timestamp: string;
}

export interface Customer {
  id: number;
  name: string;
  email?: string;
  phoneNumber?: string;
  address?: string;
  price?: number;
  createdAt: string;
  startingDate?: string;
  estimatedTime?: string;
  imageCount: number;
}

export interface Lead {
  id: number;
  name: string;
  email?: string;
  phoneNumber?: string;
  address?: string;
  source?: string;
  price?: number;
  followUpDays?: number;
  createdAt: string;
  startingDate?: string;
  estimatedTime?: string;
  imageCount: number;
}

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  getCustomers(): Observable<Customer[]> {
    return this.http.get<ApiResponse<Customer>>(`${this.baseUrl}/Customers`)
      .pipe(map(res => res.data));
  }

  getLeads(): Observable<Lead[]> {
    return this.http.get<ApiResponse<Lead>>(`${this.baseUrl}/Leads`)
      .pipe(map(res => res.data));
  }
}