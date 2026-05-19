import { createContext, useContext } from "react";

/**
 * Temporary acting-user mechanism for the pre-authentication phase.
 * Will be replaced by JWT-based identity.
 *
 * The selected user is sent as the `User-Id` header on mutating requests,
 * which the backend uses to enforce rules such as "cannot approve your own order".
 */
export interface ActingUserContextValue {
  actingUser: string;
  setActingUser: (user: string) => void;
}

export const ActingUserContext = createContext<
  ActingUserContextValue | undefined
>(undefined);

export const AVAILABLE_USERS = ["alice", "bob", "carol"] as const;

export function useActingUser(): ActingUserContextValue {
  const context = useContext(ActingUserContext);
  if (context === undefined) {
    throw new Error("useActingUser must be used within an ActingUserProvider");
  }
  return context;
}
