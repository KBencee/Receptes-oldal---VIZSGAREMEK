import React, { createContext, useEffect, useState } from "react"
import { getUserData } from "../services/protectedAPI"

type AuthUserType= {
    id: number,
    username: string,
    role: string,
    profileImageUrl: string
}

export const AuthUserContext = createContext<{authUser: AuthUserType} | undefined>(undefined)

const AuthenticatedUserContextProvider = (props: {children: React.ReactNode}) => {
    const [authUser, setAuthUser] = useState<AuthUserType>()

    useEffect(()=>{
        getUserData().then(data => {
            setAuthUser(data)})
    },[])

  return (
    <AuthUserContext.Provider value={authUser ? {authUser} : undefined}>
        {props.children}
    </AuthUserContext.Provider>
  )
}

export default AuthenticatedUserContextProvider