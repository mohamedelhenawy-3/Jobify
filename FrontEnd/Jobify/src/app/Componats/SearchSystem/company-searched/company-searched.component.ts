import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-company-searched',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './company-searched.component.html',
  styleUrl: './company-searched.component.css'
})
export class CompanySearchedComponent {
viewCompany(arg0: any) {
throw new Error('Method not implemented.');
}
  @Input() companies: any[] = [];

}
