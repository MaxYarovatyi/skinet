import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Brand } from '../shared/models/brand';
import { Pagination } from '../shared/models/pagination';
import { ProductType } from '../shared/models/productType';
import { delay, map } from 'rxjs/operators';
import { ShopParams } from '../shared/models/shopParams';
import { Product } from '../shared/models/product';
@Injectable({
  providedIn: 'root',
})
export class ShopService {
  baseUrl = 'https://localhost:5001/api/';

  constructor(private http: HttpClient) {}

  getProducts(shopParams: ShopParams) {
    let params = new HttpParams();
    if (shopParams.sort) {
      params = params.append('sort', shopParams.sort);
    }
    if (shopParams.brandId !== 0) {
      params = params.append('brandId', shopParams.brandId.toString());
    }
    if (shopParams.typeId !== 0) {
      params = params.append('typeId', shopParams.typeId.toString());
    }

    if (shopParams.search) {
      params = params.append('search', shopParams.search);
    }
    params = params.append('pageIndex', shopParams.pageNumber.toString());
    params = params.append('pageSize', shopParams.pageSize.toString());
    return this.http
      .get<Pagination>(this.baseUrl + 'products', {
        observe: 'response',
        params,
      })
      .pipe(
        map((response) => {
          return response.body;
        })
      );
  }
  getProduct(id: number) {
    return this.http.get<Product>(this.baseUrl + 'products/' + id);
  }
  getTypes() {
    return this.http.get<ProductType[]>(this.baseUrl + 'products/types');
  }
  getBrands() {
    return this.http.get<Brand[]>(this.baseUrl + 'products/brands');
  }
}
