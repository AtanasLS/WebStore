import { Component, OnInit } from '@angular/core';
import { Product } from 'src/app/shared/models/product';
import { ShopService } from '../shop.service';
import { ActivatedRoute } from '@angular/router';
import { BreadcrumbService } from 'xng-breadcrumb';
import { BasketService } from 'src/app/basket/basket.service';
import { take } from 'rxjs';

@Component({
  selector: 'app-product-details',
  templateUrl: './product-details.component.html',
  styleUrls: ['./product-details.component.scss']
})
export class ProductDetailsComponent implements OnInit {
  porduct?: Product;
  quantity = 1;
  quantityInBasket = 0;

  constructor(private shopService: ShopService, private activateRoot: ActivatedRoute, 
  private bcService: BreadcrumbService, private basketService: BasketService){
    this.bcService.set('@productDetails', ' ')
  }
  ngOnInit(): void {
    this.loadProduct();
  }

  loadProduct(){
    const id = this.activateRoot.snapshot.paramMap.get('id') 

    if (id) this.shopService.getProduct(+id).subscribe({
      next: product => {
        this.porduct = product;
        this.bcService.set('@productDetails', product.name);
        this.basketService.basketSource$.pipe(take(1)).subscribe({
          next: basket => {
            const item = basket?.items.find(x => x.id === +id);
            if (item){
              this.quantity = item.quantity;
              this.quantityInBasket = item.quantity;
            }
          }
        })
      },
      error: error => console.log(error)
    })
  }

  incrementQuantity(){
    this.quantity++;
  }
  decrementQuantity(){
    this.quantity--;
  }

  updateBasket(){
    if(this.porduct){
      if (this.quantity > this.quantityInBasket){
        const itemToAdd = this.quantity - this.quantityInBasket;
        this.quantityInBasket += itemToAdd;
        this.basketService.addItemToBasket(this.porduct, itemToAdd)
      } else {
        const itemsToRemove = this.quantityInBasket - this.quantity;
        this.quantityInBasket -= itemsToRemove;
        this.basketService.removeItemFromBasket(this.porduct.id, itemsToRemove);
      }
    }
  }


  get buttonText() {
    return this.quantityInBasket === 0 ? 'Add to basket' : 'Update basket';
  }
}
