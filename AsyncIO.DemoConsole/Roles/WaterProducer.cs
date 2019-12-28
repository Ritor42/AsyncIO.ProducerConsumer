﻿using AsyncIO.DemoConsole.Models;
using AsyncIO.ProducerConsumer.Models;
using AsyncIO.ProducerConsumer.Roles;
using HellBrick.Collections;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncIO.DemoConsole.Roles
{
    class WaterProducer : ProducerConsumer<Water, Element>
    {
        private readonly AsyncQueue<Oxigen> oxigens = new AsyncQueue<Oxigen>();
        private readonly AsyncQueue<Hidrogen> hidrogens = new AsyncQueue<Hidrogen>();

        public override bool CanConsume(object item)
        {
            return item is Oxigen || item is Hidrogen;
        }

        public override void Consume(Element item, CancellationToken token)
        {
            if (item is Oxigen oxigen)
            {
                this.oxigens.Add(oxigen);
            }
            else if (item is Hidrogen hidrogen)
            {
                this.hidrogens.Add(hidrogen);
            }
        }

        public override void Finish()
        {
            this.oxigens.Add(null);
            this.hidrogens.Add(null);
        }

        public override async Task<Water> Produce(CancellationToken token)
        {
            if(await this.oxigens.TakeAsync(token) is Oxigen)
            {
                if(await this.hidrogens.TakeAsync(token) is Hidrogen)
                {
                    if (await this.hidrogens.TakeAsync(token) is Hidrogen)
                    {
                        return new Water();
                    }
                }
            }

            this.ProducerState = State.Completed;
            return null;
        }
    }
}
