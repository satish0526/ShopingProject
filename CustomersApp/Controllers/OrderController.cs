using CustomersApp.App_Start;
using CustomersApp.Entities;
using CustomersApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CustomersApp.Controllers
{
    [RoutePrefix("api/order")]
    public class OrderController : ApiController
    {
        private static Random random = new Random();
        OrdersContext _db = new OrdersContext();

        #region Customer
        /// <summary>
        /// Based on Id, we can get the existing customer details. If Id is null / empty - it will return empty object
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("createcustomer")]
        public IHttpActionResult CreateCustomer(Guid? id)
        {
            try
            {
                CustomerModel c = new CustomerModel();
                var customer = _db.Customers.Where(v => v.Id == id).FirstOrDefault();
                if (customer != null)
                {
                    c.ContactNo = customer.ContactNo;
                    c.Id = customer.Id;
                    c.Email = customer.Email;
                    if (customer.CustomerAddresses.Any())
                        c.CustomerAddresses = customer.CustomerAddresses.Select(x => new CustomerAddressModel()
                        {
                            Id = x.Id,
                            CustomerId = x.CustomerId,
                            AddressLine1 = x.AddressLine1,
                            AddressLine2 = x.AddressLine2,
                            AddressLine3 = x.AddressLine3,
                            PostalCode = x.PostalCode,
                            Country = x.Country
                        }).ToList();
                    else
                        c.CustomerAddresses.Add(new CustomerAddressModel());
                }
                else
                {
                    c.CustomerAddresses.Add(new CustomerAddressModel());
                }
                return Ok(c);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// If CustimerId  (customerModel.Id) exist in the table, it will update the customer details. If not, it will create new customer by doing some validations.
        /// Here customer addressed are insert / update based on the record status
        /// </summary>
        /// <param name="customerModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("savecustomer")]
        [ValidationActionFilter]
        public IHttpActionResult SaveCustomer([FromBody] CustomerModel customerModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    #region validation
                    var EmailExists = _db.Customers.Where(v => v.Id != customerModel.Id && v.Email.ToLower() == customerModel.Email.ToLower()).FirstOrDefault();
                    if (EmailExists != null)
                    {
                        throw new Exception("Email already exist for other customer. Please enter different one.");
                    }
                    #endregion Validation
                    var customer = _db.Customers.Where(v => v.Id == customerModel.Id).FirstOrDefault();
                    var Addresses = customer != null ? customer.CustomerAddresses.ToList() : new List<CustomerAddress>();
                    if (customer != null)
                    {
                        customer.Name = customerModel.Name;
                        customer.ContactNo = customerModel.ContactNo;
                        customer.Email = customerModel.Email;
                        FillCustomerAddresses(customerModel, customer, Addresses);
                        _db.Entry(customer).State = EntityState.Modified;
                    }
                    else
                    {
                        customer = new Customer();
                        customer.Id = customerModel.Id = Guid.NewGuid();
                        customer.Name = customerModel.Name;
                        customer.ContactNo = customerModel.ContactNo;
                        customer.Email = customerModel.Email;
                        FillCustomerAddresses(customerModel, customer, Addresses);
                        _db.Customers.Add(customer);
                    }
                    _db.SaveChanges();
                    return Ok(customerModel);
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void FillCustomerAddresses(CustomerModel customerModel, Customer customer, List<CustomerAddress> Addresses)
        {
            CustomerAddress customerAddress = null;
            foreach (var ca in customerModel.CustomerAddresses)
            {
                if (ca.RecordStatus.ToLower() == "added")
                {
                    customerAddress = new CustomerAddress();
                    customerAddress.Id = Guid.NewGuid();
                    FillCustomerAddressModel2Entity(customerAddress, ca);
                    customerAddress.CustomerId = customer.Id;
                    _db.CustomerAddresses.Add(customerAddress);
                }
                else if (ca.RecordStatus.ToLower() == "deleted")
                {
                    customerAddress = Addresses.Where(c => c.Id == ca.Id).FirstOrDefault();
                    if (customerAddress != null)
                        _db.CustomerAddresses.Remove(customerAddress);
                }
                else if (ca.RecordStatus.ToLower() == "modified")
                {
                    customerAddress = Addresses.Where(c => c.Id == ca.Id).FirstOrDefault();
                    if (customerAddress != null)
                    {
                        FillCustomerAddressModel2Entity(customerAddress, ca);
                        _db.Entry(customerAddress).State = EntityState.Modified;
                    }
                }
            }
        }

        private static void FillCustomerAddressModel2Entity(CustomerAddress customerAddress, CustomerAddressModel ca)
        {
            customerAddress.AddressLine1 = ca.AddressLine1;
            customerAddress.AddressLine2 = ca.AddressLine2;
            customerAddress.AddressLine3 = ca.AddressLine3;
            customerAddress.PostalCode = ca.PostalCode;
            customerAddress.Country = ca.Country;
        }
        #endregion #region Customer

        #region Order
        /// <summary>
        /// To get the order based on Id
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("createorder")]
        public IHttpActionResult CreateOrder(Guid? id)
        {
            try
            {
                Order order = _db.Orders.Where(c => c.Id == (id ?? Guid.Empty)).FirstOrDefault();
                OrderModel or = new OrderModel();
                if (order != null)
                {
                    or.Id = order.Id;
                    or.RefNumber = order.RefNumber;
                    or.Remarks = order.Remarks;
                    or.OrderDetailModels = order.OrderDetails.Select(c => new OrderDetailModel()
                    {
                        Id = c.Id,
                        ItemName = c.Item.Name,
                        ItemId = c.ItemId,
                        OrderId = c.OrderId,
                        Quantity = c.Quantity,
                        GrandTotal = c.Quantity * c.Item.UnitPrice
                    }).ToList();
                }
                else
                {
                    or.OrderDetailModels.Add(new OrderDetailModel());
                }
                return Ok(or);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// TO ssave the order details
        /// Here we maintained the items as seed data in Exam.Item table.
        /// From UI, need to pass the ItemId.
        /// </summary>
        /// <param name="orderModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("saveorder")]
        [ValidationActionFilter]
        public IHttpActionResult SaveOrder(OrderModel orderModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Order order = _db.Orders.Where(c => c.Id == orderModel.Id).FirstOrDefault();
                    if (order != null)
                    {
                        order.RefNumber = RandomString(8);
                        order.Remarks = orderModel.Remarks;
                        FillOrderDetails(orderModel, order);
                        _db.Entry(order).State = EntityState.Modified;
                    }
                    else
                    {
                        order = new Order();
                        order.Id = orderModel.Id= Guid.NewGuid();
                        order.CustomerId = orderModel.CustomerId;
                        order.RefNumber = RandomString(8);
                        order.Remarks = orderModel.Remarks;
                        order.CreatedDate = DateTime.UtcNow;
                        FillOrderDetails(orderModel, order);
                        _db.Orders.Add(order);
                    }
                    _db.SaveChanges();
                    return Ok(orderModel);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            return null;
        }

        private void FillOrderDetails(OrderModel orderModel, Order order)
        {
            OrderDetail orderDetail = null;
            foreach (var od in orderModel.OrderDetailModels)
            {
                if (od.RecordStatus.ToLower() == "added")
                {
                    orderDetail = new OrderDetail();
                    orderDetail.Id = Guid.NewGuid();
                    FillOrderDetailModel2Entity(order, orderDetail, od);
                    _db.OrderDetails.Add(orderDetail);
                }
                else if (od.RecordStatus.ToLower() == "deleted")
                {
                    orderDetail = order.OrderDetails.Where(c => c.Id == od.Id).FirstOrDefault();
                    if (orderDetail != null)
                        _db.OrderDetails.Remove(orderDetail);
                }
                else if (od.RecordStatus.ToLower() == "modified")
                {
                    orderDetail = order.OrderDetails.Where(c => c.Id == od.Id).FirstOrDefault();
                    if (orderDetail != null)
                    {
                        FillOrderDetailModel2Entity(order, orderDetail, od);
                        _db.Entry(orderDetail).State = EntityState.Modified;
                    }
                }
            }
        }

        private static void FillOrderDetailModel2Entity(Order order, OrderDetail orderDetail, OrderDetailModel od)
        {
            orderDetail.ItemId = od.ItemId;
            orderDetail.OrderId = order.Id;
            orderDetail.Quantity = od.Quantity;
        }

        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        #endregion Order

        #region OrderList
        /// <summary>
        /// By passing the Index and Page size, we can get the all orders for that specific customer
        /// </summary>
        /// <param name="customrId"></param>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getallcustomerorders")]
        public IHttpActionResult GetOrders(Guid customrId, int index, int pageSize)
        {
            try
            {
                var orders = _db.Orders.Where(c => c.CustomerId == customrId).OrderByDescending(c => c.CreatedDate).ToList().Take(index * pageSize).Skip((index - 1) * pageSize);
                var lstOrderModels = orders.Select(c => new OrderModel()
                {
                    Id = c.Id,
                    CustomerId = c.CustomerId,
                    RefNumber = c.RefNumber,
                    Remarks = c.Remarks,
                    OrderDetailModels = c.OrderDetails.Select(x => new OrderDetailModel()
                    {
                        Id = x.Id,
                        ItemId = x.ItemId,
                        ItemName = x.Item.Name,
                        OrderId = x.OrderId,
                        Quantity = x.Quantity,
                        GrandTotal = x.Item.UnitPrice * x.Quantity
                    }).ToList()
                }).ToList();
                return Ok(lstOrderModels);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion OrderList

    }
}
