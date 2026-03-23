import { faMagnifyingGlass } from '@fortawesome/free-solid-svg-icons'
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import styles from '../css/Headbar.module.css'
import type { RecipeType } from '../types/RecipeTypes';
import { searchRecipes } from '../queryOptions/createRecipeQueryOption';


const HeadBar = ({ onSearch }: { onSearch: (results: RecipeType[]) => void }) => {

    const handleSearch = async (term: string) => {
    const results = await searchRecipes(term)
    onSearch(results)
  }

  return (
    <div className={styles.headBar}>
        <h1>Receptek</h1>
        {/* <p>Ha nincs ötlet a főzéshez...<p>
        <button onClick={searchButtonClick}>Keresés...<FontAwesomeIcon icon={faMagnifyingGlass} /></button> */}
        <input onChange={(e) => handleSearch(e.target.value)} />
    </div>
  )
}

export default HeadBar