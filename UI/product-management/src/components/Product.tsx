import {
  DefaultButton,
  DirectionalHint,
  FocusTrapCallout,
  Image,
  ImageFit,
  Label,
  mergeStyles,
  Stack,
  StackItem
} from "@fluentui/react";
import { useId } from "@fluentui/react-hooks";
import React from "react";
import { ProductViewModel } from "../models/ProductViewModel";
import { QuantitySelector } from "./QuantitySelector";

interface IProductProps {
  product: ProductViewModel
}

export const Product = (props: IProductProps) => {
  const [isCalloutVisible, setPopupVisible] = React.useState(false);

  const popupStyles = mergeStyles({
    width: 250,
    padding: "20px"
  });

  const buttonId = useId();

  return (
    <Stack horizontalAlign="center" tokens={{ childrenGap: 10 }}>
      <Label>{props.product.name}</Label>
      <Image
        src={new URL(props.product.imageUrl, import.meta.url).toString()}
        styles={{ root: { border: "1px solid black" } }}
        imageFit={ImageFit.contain}
        width={150}
        height={150}
      />
      <StackItem align="end">
        <DefaultButton id={buttonId} onClick={() => setPopupVisible(true)}>
          Add
        </DefaultButton>
      </StackItem>

      {isCalloutVisible && (
        <FocusTrapCallout
          className={popupStyles}
          role="alertdialog"
          target={`#${buttonId}`}
          directionalHint={DirectionalHint.rightBottomEdge}
          onDismiss={() => setPopupVisible(false)}
          setInitialFocus
        >
          <QuantitySelector 
            unitOfMeasures={props.product.unitOfMeasures}
            productName={props.product.name}
            onClose={() => setPopupVisible(false)} />
        </FocusTrapCallout>
      )}
    </Stack>
  );
};
