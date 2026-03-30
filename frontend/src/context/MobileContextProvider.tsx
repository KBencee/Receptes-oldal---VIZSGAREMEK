import { createContext, useContext, useState, type ReactNode } from "react"

type MobileContextType = {
    isMobile: boolean
}

const MobileContext = createContext<MobileContextType | undefined>(undefined)

const MobileContextProvider = ({children}:{children: ReactNode}) => {
    const [isMobile, setIsMobile] = useState(window.innerWidth < 768)

    const switchToMobile = () => {
        if(isMobile !== window.innerWidth <= 768)
            setIsMobile(window.innerWidth <= 768)
    }
    window.addEventListener("resize", switchToMobile)

  return (
        <MobileContext.Provider value={{isMobile}}>
            {children}
        </MobileContext.Provider>
    )
}

export const useMobileContext = () => {
    const context = useContext(MobileContext)
    if (!context){
        throw new Error("No mobile context!")
    }
    return context
}

export default MobileContextProvider