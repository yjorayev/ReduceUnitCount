import { Text, Stack, StackItem } from "@fluentui/react";
import React from "react";
import { CartContext } from "../contexts/CartContext";
import { CartItem } from "../models/CartItem";

export const Cart = () => {
  const cart = React.useContext(CartContext);
const [toShip, setToShip] = React.useState<CartItem[]>([]);

  React.useEffect(() => {

    const fetch = (cartItems: CartItem[]) => {
        //should call api here
        return cartItems.map(item => ({
            name: 'FAKE',
            unitOfMeasureName: 'FAKE UOM',
            quantity: 10000
        }));
    }

    setToShip(fetch(cart.items));
  }, [cart.items]);

  return (
    <Stack style={{ margin: 10 }} horizontal tokens={{childrenGap: 100}} horizontalAlign="center">
      <StackItem>
        <Text variant={"xLargePlus"}>Selected items: </Text>
        {cart.items.map((item, idx) => (
          <Stack key={`cartItem${idx}`} style={{ margin: 10 }} tokens={{ childrenGap: 5 }}>
            <Text variant={"medium"}>Name: {item.name}</Text>
            <Text variant={"medium"}>UOM: {item.unitOfMeasureName}</Text>
            <Text variant={"medium"}>Quantity: {item.quantity}</Text>
          </Stack>
        ))}
      </StackItem>
      <StackItem>
        <Text variant={"xLargePlus"}>Items to ship: </Text>
        {toShip.map((item, idx) => (
          <Stack key={`cartItem${idx}`} style={{ margin: 10 }} tokens={{ childrenGap: 5 }}>
            <Text variant={"medium"}>Name: {item.name}</Text>
            <Text variant={"medium"}>UOM: {item.unitOfMeasureName}</Text>
            <Text variant={"medium"}>Quantity: {item.quantity}</Text>
          </Stack>
        ))}
      </StackItem>
    </Stack>
  );
};
