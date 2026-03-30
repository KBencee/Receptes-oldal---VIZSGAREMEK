import { Link } from "react-router-dom"
import { menuItems, setActive } from "./NavMiddle";
import styles from '../css/Nav.module.css'

const DesktopNavLinks = () => {

  return (
    <>
      {menuItems.map(x => <Link to={x.link} className={x.active ? styles.active : ""} onClick={() => setActive(x.name)} key={x.id}>{x.name}</Link>)}
    </>
  )
}

export default DesktopNavLinks