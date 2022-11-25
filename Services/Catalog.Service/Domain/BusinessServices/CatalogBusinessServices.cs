using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.API.Contracts;
using Catalog.API.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Catalog.API.Domain.BusinessServices
{
    public class CatalogBusinessServices : ICatalogBusinessServices
    {
        private readonly IArtistRepository _artistRepository;
        private readonly IGenreRepository _genreRepository;
        private readonly ILogger<CatalogBusinessServices> _logger;
        private readonly IMusicRepository _musicRepository;

        public CatalogBusinessServices(IMusicRepository musicRepository,
            IGenreRepository genreRepository,
            IArtistRepository artistRepository,
            ILogger<CatalogBusinessServices> logger)
        {
            _musicRepository = musicRepository;
            _genreRepository = genreRepository;
            _artistRepository = artistRepository;
            _logger = logger;
        }

        public async Task<List<Product>> GetAllMusic(string correlationToken)
        {
            return await _musicRepository.GetAll(correlationToken);
        }

        public async Task<Product> GetMusic(string correlationToken, int albumId)
        {
            return await _musicRepository.GetById(albumId, correlationToken);
        }

        public async Task<List<Product>> GetTopSellingMusic(string correlationToken, int count)
        {
            return await _musicRepository.GetTopSellers(count, correlationToken);
        }

        public async Task<List<Genre>> GetAllGenres(string correlationToken, bool includeAlbums = false)
        {
            return includeAlbums
                ? await _genreRepository.GetAllAndAlbums(correlationToken)
                : await _genreRepository.GetAll(correlationToken);
        }

        public async Task<Genre> GetGenre(int genreId, string correlationToken, bool includeAlbums = false)
        {
            return await _genreRepository.GetById(genreId, correlationToken, includeAlbums);
        }

        public async Task<List<Artist>> GetAllArtists(string correlationToken)
        {
            return await _artistRepository.GetAll(correlationToken);
        }

        public async Task<Artist> GetArtist(int artistID, string correlationToken)
        {
            return await _artistRepository.GetById(artistID, correlationToken);
        }

        public async Task Add(string correlationToken, Product product)
        {
            // Idempotent write check. Ensure insert with same correlation token has
            // not already happened. This would most likely do to a retry after the
            // product has been added.
            var targetAlbum = await _musicRepository.GetByIdWithIdempotencyCheck(product.Id, correlationToken);

            if (targetAlbum == null)
            {
                // Product has not been added yet
                await _musicRepository.Add(product);

                // Hack: Yet another transformation of same data.
                //       Added to remove issue in new Core Serializer which doesn't allow circular references.
                var productUpsert = new ProductUpsert
                {
                    Id = product.Id,
                    ArtistId = product.ArtistId,
                    GenreId = product.GenreId,
                    Title = product.Title,
                    ParentalCaution = product.ParentalCaution,
                    Cutout = product.Cutout,
                    Price = product.Price,
                    ReleaseDate = product.ReleaseDate,
                    Upc = product.Upc
                };
            }
        }

        public async Task Update(string correlationToken, Product product)
        {
            await _musicRepository.Update(product);

            // Hack: Yet another transformation of same data.
            //       Added to remove issue in new Core Serializer which doesn't allow circular references.
            var productUpsert = new ProductUpsert
            {
                Id = product.Id,
                ArtistId = product.ArtistId,
                GenreId = product.GenreId,
                Title = product.Title,
                ParentalCaution = product.ParentalCaution,
                Cutout = product.Cutout,
                Price = product.Price,
                ReleaseDate = product.ReleaseDate,
                Upc = product.Upc
            };
            //************** Publish Event  *************************
        }
    }
}