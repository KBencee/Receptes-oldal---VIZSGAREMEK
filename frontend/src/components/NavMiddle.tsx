import { useState } from 'react'
import MobileNavLinks from './MobileNavLinks'
import DesktopNavLinks from './DesktopNavLinks'
import { faBars, faX } from '@fortawesome/free-solid-svg-icons'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import styles from '../css/Nav.module.css'
import { useMobileContext } from '../context/MobileContextProvider'

export type linkType = {
    id: number,
    name: string,
    link: string,
    active: boolean
}

export const menuItems = [
    {id: 0, name: "Kezdőlap", link: "/", active: true},
    {id: 1, name: "Saját receptek", link: "/own", active: false},
    {id: 2, name: "Feltöltés", link: "/upload", active: false},
    {id: 3, name: "Mentve", link: "/saved", active: false},
]

export const setActive = (activeItem:string) => {
    menuItems.forEach(e => {
        e.name == activeItem ? e.active = true : e.active = false
    });
}

const NavMiddle = () => {
    const [isToggled, setIsToggled] = useState(true)
    const { isMobile } = useMobileContext()

  return (
    <div className={styles.middle}>
        {isMobile && <button onClick={() => setIsToggled(!isToggled)}> {isToggled ? <FontAwesomeIcon icon={faBars}/> : <FontAwesomeIcon icon={faX} />}</button>}
        <nav className={(isMobile ? styles.mobileNav : styles.desktopNav) + " " + (isMobile ? isToggled ? styles.toggled : styles.open : "")} >
            {isMobile ? <MobileNavLinks/> : <DesktopNavLinks/>}
        </nav>
    </div>
  )
}

export default NavMiddle