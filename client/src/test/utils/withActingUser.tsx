import type { ReactNode } from "react";
import { ActingUserContext } from "@/features/auth/actingUser";

interface Props {
  user?: string;
  children: ReactNode;
}

export function WithActingUser({ user = "bob", children }: Props) {
  return (
    <ActingUserContext.Provider
      value={{ actingUser: user, setActingUser: () => {} }}
    >
      {children}
    </ActingUserContext.Provider>
  );
}
