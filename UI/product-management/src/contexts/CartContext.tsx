import React from "react";
import { Cart } from "../models/Cart";

export const CartContext = React.createContext<Cart>({items:[]});

export const CartContextProvider = (props: any) => {
    const contextValue = {
        items: [],
    };
  return <CartContext.Provider value={contextValue}>{props.children}</CartContext.Provider>;
};
