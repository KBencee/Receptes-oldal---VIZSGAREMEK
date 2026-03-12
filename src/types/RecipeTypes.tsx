export type RecipeType = {
  id: string;
  nev: string;
  leiras: string;
  hozzavalok: string;
  elkeszitesiIdo: number;
  nehezsegiSzint: string;
  likes: number;
  feltoltoUsername: string;
  kepUrl: string;
  cimkek: string[];
  mentveVan: boolean;
  likeolvaVan: boolean;
};

export type CommentType = {
  id: string;
  szoveg: string;
  irtaEkkor: string;
  username: string;
  userId: string;
  sajatKomment: boolean;
};
