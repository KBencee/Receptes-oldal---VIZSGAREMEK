import NavMiddle from './NavMiddle'
import styles from '../css/Nav.module.css'
import Profile from './Profile'
import { useMobileContext } from '../context/MobileContextProvider'

const Navbar = () => {
    const { isMobile } = useMobileContext()
  return (
    <div className={styles.navbar} >
        {isMobile ? "" : <div><img src="my-recipes-logo.png" alt="logo" className={styles.logo}/></div>}
        <NavMiddle/>
        <Profile/>
    </div>
  )
}

export default Navbar