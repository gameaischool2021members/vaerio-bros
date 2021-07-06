"""
A categorical VAE for creating
Super Mario Bros levels.

Author: Miguel González Duque
License: MIT. Attribute me, though :)
"""
from typing import List

import torch
import torch.nn as nn

Tensor = torch.Tensor


class View(nn.Module):
    def __init__(self, shape):
        super(View, self).__init__()
        self.shape = shape

    def forward(self, x):
        return x.view(*self.shape)


class VAEMario(nn.Module):
    def __init__(
        self,
        z_dim: int,
        n_sprites: int = 11,
        h_dims: List[int] = None,
    ):
        super(VAEMario, self).__init__()
        self.w = 14
        self.h = 14
        self.n_sprites = n_sprites
        self.input_dim = 14 * 14 * n_sprites

        self.z_dim = z_dim or 64
        self.h_dims = h_dims or [256, 128]

        # Adding the input layer with onehot encoding
        # (assuming that the views are inside the net)
        self.h_dims = [self.input_dim] + self.h_dims
        modules = []
        for dim_1, dim_2 in zip(self.h_dims[:-1], self.h_dims[1:]):
            if dim_1 == self.h_dims[0]:
                modules.append(
                    nn.Sequential(
                        View([-1, self.input_dim]), nn.Linear(dim_1, dim_2), nn.Tanh()
                    )
                )
            else:
                modules.append(nn.Sequential(nn.Linear(dim_1, dim_2), nn.Tanh()))

        self.encoder = nn.Sequential(*modules)
        self.fc_mu = nn.Sequential(nn.Linear(self.h_dims[-1], z_dim))
        self.fc_var = nn.Sequential(nn.Linear(self.h_dims[-1], z_dim))

        dec_dims = self.h_dims.copy() + [z_dim]
        dec_dims.reverse()
        dec_modules = []
        for dim_1, dim_2 in zip(dec_dims[:-1], dec_dims[1:]):
            if dim_2 != dec_dims[-1]:
                dec_modules.append(nn.Sequential(nn.Linear(dim_1, dim_2), nn.Tanh()))
            else:
                dec_modules.append(
                    nn.Sequential(
                        nn.Linear(dim_1, dim_2),
                        View([-1, self.n_sprites, self.h, self.w]),
                        nn.LogSoftmax(dim=1),
                    )
                )

        self.decoder = nn.Sequential(*dec_modules)

    def encode(self, x: Tensor) -> List[Tensor]:
        result = self.encoder(x)
        mu = self.fc_mu(result)
        log_var = self.fc_var(result)

        return [mu, log_var]

    def decode(self, z: Tensor) -> Tensor:
        result = self.decoder(z)
        return result

    def reparametrize(self, mu: Tensor, log_var: Tensor) -> Tensor:
        std = torch.exp(0.5 * log_var)
        rvs = torch.randn_like(std)

        return rvs.mul(std).add_(mu)

    def forward(self, x: Tensor) -> List[Tensor]:
        # Does a forward pass through the network.
        mu, log_var = self.encode(x.view(-1, self.input_dim))

        # Sample z from p(z|x)
        z = self.reparametrize(mu, log_var)

        # Decode this z
        x_prime = self.decode(z)

        return [x_prime, x, mu, log_var]
