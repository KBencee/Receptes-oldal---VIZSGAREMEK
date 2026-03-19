import { useMobileContext } from '../context/MobileContextProvider'
import HomeBtn from './HomeBtn'
import styles from '../css/ForYou.module.css'
import { Link } from 'react-router-dom'
import type { RecipeType } from '../types/RecipeTypes'
import Like from './Like'
import Save from './Save'

const ForYouReel = (recipe:RecipeType) => {
    const {isMobile} = useMobileContext()

    return (
    <section className={isMobile ? styles.mobileSection : ""}>
        <div className={styles.forYouHead}><HomeBtn/><span>{recipe.nev}</span></div>
        {recipe.kepUrl.length > 0 && <img src={recipe.kepUrl} />}
        <aside>
            <Like {...recipe}/>
            <Save {...recipe}/>
            <i className="fa-solid fa-share"></i>
            {isMobile ? <Link to={"/description/" + recipe.id}><i className="fa-solid fa-ellipsis"></i></Link> : ""}
        </aside>
        <div className={styles.tags}>
            {recipe.cimkek.map((c,idx) => <span key={idx} className={styles.tag}>{c}</span>)}
        </div>
    </section>
  )
}

export default ForYouReel