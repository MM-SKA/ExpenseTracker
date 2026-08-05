import { Component, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-scroll-top',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './scroll-top.html',
  styleUrl: './scroll-top.css'
})
export class ScrollTopComponent {

  public showButton = false;

  @HostListener('window:scroll')
  public onWindowScroll(): void {

    this.showButton = window.scrollY > 300;

  }

  public scrollToTop(): void {

    window.scrollTo({
      top: 0,
      behavior: 'smooth'
    });

  }

}
