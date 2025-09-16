import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ImageGalleryDialogComponent } from '../image-gallery-dialog/image-gallery-dialog.component';

@Component({
  selector: 'app-leads-cards',
  imports: [CommonModule],
  templateUrl: './leads.component.html',
  styleUrl: './leads.component.scss'
})
export class LeadsComponent {
  @Input() leads: any[] = [];

  constructor(
    private dialog: MatDialog
  ){}
  
  openGallery(entityId: number, name: string) {
    this.dialog.open(ImageGalleryDialogComponent, {
      width: '90vw',
      data: { entityId, entityType: 'leads', name: name }
    });
  } 
}
