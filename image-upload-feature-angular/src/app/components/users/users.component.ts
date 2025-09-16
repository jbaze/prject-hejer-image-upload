import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CustomersComponent } from './customers/customers.component';
import { LeadsComponent } from './leads/leads.component';
import { HttpClientModule } from '@angular/common/http';
import { Customer, Lead, UserService } from '../../services/users.service';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [CommonModule, HttpClientModule, CustomersComponent, LeadsComponent],
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.scss']
})
export class UsersComponent implements OnInit {
  selectedTab: 'customers' | 'leads' = 'customers';
  customers: Customer[] = [];
  leads: Lead[] = [];
  loadingCustomers: boolean = false;
  loadingLeads: boolean = false;

  constructor(private userService: UserService) {}

  ngOnInit() {
    this.loadCustomers();
    this.loadLeads();
  }

  selectTab(tab: 'customers' | 'leads') {
    this.selectedTab = tab;
  }

  loadCustomers() {
    this.loadingCustomers = true;
    this.userService.getCustomers().subscribe({
      next: (data) => { this.customers = data; this.loadingCustomers = false; },
      error: () => { this.loadingCustomers = false; console.error('Failed to load customers'); }
    });
  }

  loadLeads() {
    this.loadingLeads = true;
    this.userService.getLeads().subscribe({
      next: (data) => { this.leads = data; this.loadingLeads = false; },
      error: () => { this.loadingLeads = false; console.error('Failed to load leads'); }
    });
  }
}