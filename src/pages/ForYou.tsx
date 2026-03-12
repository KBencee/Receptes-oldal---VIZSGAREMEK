import { useNavigate, useParams } from "react-router-dom"
import { createNextRecipeQueryOption, createPrevRecipeQueryOption, createRecipeByIdQueryOption, type RecipeType } from "../queryOptions/createRecipeQueryOption"
import { useMobileContext } from "../context/MobileContextProvider"
import { useCallback, useState } from "react"
import { useQuery } from "@tanstack/react-query"
import styles from '../css/ForYou.module.css'
import ForYouReel from "../components/ForYouReel"
import Description from "../components/Description"

const ForYou = () => {
    const navigate = useNavigate()
    const {isMobile} = useMobileContext()
    const {id} = useParams()

    const mock: RecipeType = {cimkek:[],elkeszitesiIdo:0,feltoltoUsername:"",hozzavalok:"",id:"",kepUrl:"",leiras:"",likeolvaVan:false,likes:0,mentveVan:false, nehezsegiSzint:"",nev:""}

    const {data} = useQuery(createRecipeByIdQueryOption(id as string))

    const recipe: RecipeType = data as RecipeType

    const next = useQuery({
        ...createNextRecipeQueryOption(data?.id as string),
        enabled: !!data
    })

    const prev = useQuery({
        ...createPrevRecipeQueryOption(data?.id as string),
        enabled: !!data
    })

    const [touchStart, setTouchStart] = useState<{ x: number } | null>(null);
    const [touchEnd, setTouchEnd] = useState<{ x: number } | null>(null);

    const handleTouchStart = useCallback((e: React.TouchEvent) => {
        setTouchEnd(null);
        setTouchStart({
            x: e.touches[0].clientX,
        });
    }, []);

    const handleTouchMove = useCallback((e: React.TouchEvent) => {
        setTouchEnd({
            x: e.touches[0].clientX,
        });
    }, []);

    const handleTouchEnd = useCallback(() => {
        if (!touchStart || !touchEnd) return;
        if(!next || !prev) return;

        const xDiff = touchStart.x - touchEnd.x;

        if (xDiff > 0 && next.data) {
            nextFuncion()
        } else if (xDiff < 0 && prev.data) {
            prevFuncion()
        }

        setTouchStart(null);
        setTouchEnd(null);
    }, [touchStart, touchEnd, next, prev]);

    const nextFuncion = () => {
        if(next?.data)
            navigate("/foryou/" + next.data.id)
    }
    const prevFuncion = () => {
        if(prev?.data)
            navigate("/foryou/" + prev.data.id)
    }    

  return (
    <>
    {recipe && next && prev ? 
        <div className={styles.forYou}
            onTouchStart={handleTouchStart}
            onTouchMove={handleTouchMove}
            onTouchEnd={handleTouchEnd}
        >
            <ForYouReel {...recipe}/>
            { !isMobile &&
                <>
                    <div className={styles.arrows}>
                        <i className="fa-solid fa-circle-down fa-rotate-180 fa-xl" onClick={() => {nextFuncion()}}></i>
                        <i className="fa-solid fa-circle-down fa-xl" onClick={() => {prevFuncion()}}/>
                    </div>
                    <Description {...recipe}/>
                </>
            }
        </div>: 
        <div className={styles.forYou}>
            <ForYouReel {...mock}/>
            { !isMobile &&
                <>
                    <div className={styles.arrows}>
                        <i className="fa-solid fa-circle-down fa-rotate-180 fa-xl"></i>
                        <i className="fa-solid fa-circle-down fa-xl"/>
                    </div>
                    <Description {...mock}/>
                </>
            }
        </div>
    }
    </>
  )
}

export default ForYou