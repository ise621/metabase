import { GraphQLFormattedError } from "graphql";
import { FormInstance } from "antd";
import { Dispatch, SetStateAction } from "react";

export function handleFormErrors(
  graphQlErrors: readonly GraphQLFormattedError[] | undefined,
  userErrors: { code: string; message: string; path: string[] }[] | undefined,
  setGlobalErrorMessages: Dispatch<SetStateAction<string[]>>,
  form: FormInstance<any>
) {
  const globalErrorMessages = new Array<string>();
  if (graphQlErrors) {
    // TODO Is this how we want to handle GraphQl errors?
    globalErrorMessages.push(...graphQlErrors.map((e) => e.message));
  }
  if (userErrors) {
    const errorPathToMessage = userErrors.reduce((a, x) => {
      // We use strings as keys instead of path arrays because the
      // latter are compared by reference.
      const pathAsString = x.path.join(".");
      if (!a.has(pathAsString)) {
        a.set(pathAsString, [x.path, []]);
      }
      a.get(pathAsString)?.[1]?.push(x.message);
      return a;
    }, new Map<string, [string[], string[]]>());
    console.log(errorPathToMessage);
    for (let [, [path, messages]] of errorPathToMessage) {
      if (path.length === 1) {
        globalErrorMessages.push(...messages);
      } else {
        form.setFields([
          {
            name: path.slice(1),
            errors: messages,
          },
        ]);
      }
    }
  }
  setGlobalErrorMessages(globalErrorMessages);
}
