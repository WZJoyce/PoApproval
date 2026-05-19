import { useState, type ReactNode } from "react";
import { ActingUserContext, AVAILABLE_USERS } from "./actingUser";

export function ActingUserProvider({ children }: { children: ReactNode }) {
  const [actingUser, setActingUser] = useState<string>(AVAILABLE_USERS[0]);

  return (
    <ActingUserContext.Provider value={{ actingUser, setActingUser }}>
      {children}
    </ActingUserContext.Provider>
  );
}
