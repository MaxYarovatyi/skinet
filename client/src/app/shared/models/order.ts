import { IAdress } from './adress';

export interface IOrderToCreate {
  basketId: string;
  deliveryMethodId: number;
  shipToAddress: IAdress;
}

export interface IOrderItem {
  productId: number;
  productName: string;
  pictureUrl: string;
  price: number;
  quantity: number;
}

export interface IOrder {
  id: number;
  buyerEmail: string;
  orderDate: Date;
  shipToAddress: IAdress;
  deliveryMethod: string;
  shippinPrice: number;
  orderItems: IOrderItem[];
  subtotal: number;
  status: string;
  total: number;
}
